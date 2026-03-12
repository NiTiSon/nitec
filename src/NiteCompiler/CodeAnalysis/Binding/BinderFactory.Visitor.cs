using System.Diagnostics;
using NiteCompiler.CodeAnalysis.Symbols;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Compilation;

namespace NiteCompiler.CodeAnalysis.Binding;

internal partial class BinderFactory
{
	internal enum NodeUsage
	{
		Normal = 0,
		FunctionGenericParameters = 1 << 0,
		FunctionBody = 1 << 1,
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

		public override Binder Visit(SyntaxNode node)
		{
			return VisitCore(node);
		}

		private Binder VisitCore(SyntaxNode node)
		{
			return node.Accept(this)!;
		}

		public override Binder? VisitFunctionDeclaration(FunctionDeclarationSyntax declaration)
		{
			if (!LookupPosition.IsInMethodDeclaration(_position, declaration))
			{
				return VisitCore(declaration.Parent);
			}

			NodeUsage usage;
			if (LookupPosition.IsInBody(_position, declaration))
			{
				usage = NodeUsage.MethodBody;
			}
			else if (LookupPosition.IsInMethodTypeParameterScope(_position, methodDecl))
			{
				usage = NodeUsage.MethodTypeParameters;
			}
			else
			{
				usage = NodeUsage.Normal;
			}
		}
	}
}