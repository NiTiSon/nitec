using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Symbols.Source;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.CodeAnalysis.Text;
using NiteCompiler.Compilation;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class BinderFactory
{
	internal enum NodeUsage
	{
		Normal = 0,
		FunctionGenericParameters = 1 << 0,
		FunctionBody = 1 << 1,

		ModuleBody = 1 << 0,
	}

	internal sealed class Visitor : SyntaxVisitor<Binder>
	{
		private int _position;
		private SyntaxNode? _memberDeclaration;
		private Symbol? _member;
		private BinderFactory _factory = null!;

		internal void Initialize(BinderFactory factory, int position, SyntaxNode? memberDeclaration, Symbol? member)
		{
			Debug.Assert((memberDeclaration == null) == (member == null));

			_factory = factory;
			_position = position;
			_memberDeclaration = memberDeclaration;
			_member = member;
		}

		internal void Clear()
		{
			_factory = null!;
			_position = 0;
			_memberDeclaration = null;
			_member = null;
		}

		private NiteCompilation Compilation => _factory._compilation;
		private SyntaxTree SyntaxTree => _factory._syntaxTree;
		private SeniorBinder SeniorBinder => _factory._seniorBinder;
		private Dictionary<BinderCache, Binder> BinderCache => _factory._binderCache;

		protected override Binder DefaultVisit(SyntaxNode parent)
		{
			return VisitCore(parent.Parent!);
		}

		public override Binder Visit(SyntaxNode node)
		{
			return VisitCore(node);
		}

		private Binder VisitCore(SyntaxNode node)
		{
			return node.Accept(this)!;
		}

		public override Binder VisitCompilationUnit(CompilationUnitSyntax compilationUnit)
		{
			if (compilationUnit != SyntaxTree.Root)
			{
				throw new ArgumentOutOfRangeException(nameof(compilationUnit), "node not part of tree");
			}

			var key = new BinderCache(compilationUnit, NodeUsage.Normal);
			if (!BinderCache.TryGetValue(key, out Binder? result))
			{
				result = SeniorBinder;

				var globalModule = Compilation.SourceLibrary.GlobalModule;
				result = new InContainerBinder(globalModule, result);
			}

			return result;
		}

		public override Binder VisitModuleDeclaration(ModuleDeclarationSyntax declaration)
		{
			if (!LookupPosition.IsInModuleDeclaration(_position, declaration))
			{
				return VisitCore(declaration.Parent!);
			}

			NodeUsage usage;
			if (LookupPosition.IsInBody(_position, declaration))
			{
				usage = NodeUsage.ModuleBody;
			}
			else
			{
				usage = NodeUsage.Normal;
			}

			var key = new BinderCache(declaration, usage);

			if (!BinderCache.TryGetValue(key, out Binder? resultBinder))
			{
				resultBinder = VisitCore(declaration.Parent!);

				if (usage == NodeUsage.ModuleBody)
				{
					resultBinder = MakeModuleBinder(declaration, resultBinder);
				}

				BinderCache.TryAdd(key, resultBinder);
			}

			return resultBinder;
		}

		private Binder MakeModuleBinder(ModuleDeclarationSyntax declaration, Binder outer)
		{
			Binder binder = outer;

			ModuleSymbol container = Compilation.SourceLibrary.GlobalModule;

			foreach (var namePart in declaration.Name.Parts)
			{
				var name = namePart.GetName();

				container = container.GetNestedModule(name)!;
				Debug.Assert(container != null);

				binder = new InContainerBinder(container, binder);
			}

			return binder;
		}

		public override Binder? VisitFunctionDeclaration(FunctionDeclarationSyntax declaration)
		{
			if (!LookupPosition.IsInFunctionDeclaration(_position, declaration))
			{
				return VisitCore(declaration.Parent!);
			}

			NodeUsage usage;
			if (LookupPosition.IsInBody(_position, declaration.Body))
			{
				usage = NodeUsage.FunctionBody;
			}
			else
			{
				usage = NodeUsage.Normal;
			}

			var key = new BinderCache(declaration, usage);

			if (!BinderCache.TryGetValue(key, out Binder? resultBinder))
			{
				resultBinder = VisitCore(declaration.Parent!);

				SourceFunctionSymbol? function = null;
				// if (usage != NodeUsage.Normal && declaration.GenericParameterList != null)
				// {
				// 	method = GetFunctionSymbol(declaration, resultBinder);
				// 	resultBinder = new WithFunctionGenericParametersBinder(function, resultBinder);
				// }
				if (usage == NodeUsage.FunctionBody)
				{
					function = function ?? GetFunctionSymbol(declaration, resultBinder);
					resultBinder = new InFunctionBinder(function, resultBinder);
				}

				BinderCache.TryAdd(key, resultBinder);
			}

			return resultBinder;
		}

		private ContainerSymbol? GetContainer(Binder binder, SyntaxNode node)
		{
			Symbol? containingSymbol = binder.ContainingMember;

			if (containingSymbol is not ContainerSymbol container)
			{
				// ContainingMember should always be either a module or a type
				throw new UnreachableException();
			}

			return container;
		}

		private SourceFunctionSymbol? GetFunctionSymbol(FunctionDeclarationSyntax baseFunctionDeclarationSyntax, Binder outerBinder)
		{
			if (baseFunctionDeclarationSyntax == _memberDeclaration)
			{
				return (_member as SourceFunctionSymbol)!;
			}

			ContainerSymbol? container = GetContainer(outerBinder, baseFunctionDeclarationSyntax);
			if (container == null) return null;

			var members = container.GetMembers();
			foreach (var member in members)
			{
				if (member is SourceFunctionSymbol function)
				{
					if (function.Syntax == baseFunctionDeclarationSyntax)
					{
						return function;
					}
				}
			}

			return null;
		}
	}
}