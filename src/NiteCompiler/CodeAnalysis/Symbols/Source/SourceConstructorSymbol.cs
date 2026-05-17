using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using NiteCompiler.CodeAnalysis.Binding;
using NiteCompiler.CodeAnalysis.Syntax;
using NiteCompiler.Diagnostics;

namespace NiteCompiler.CodeAnalysis.Symbols.Source;

internal sealed class SourceConstructorSymbol : ConstructorSymbol
{
	public override TypeSymbol ContainingSymbol { get; }
	public BaseConstructorDeclarationSyntax Syntax { get; }
	public override string Name { get; }

	public override ImmutableArray<ParameterSymbol> Parameters
	{
		get
		{
			if (field.IsDefault)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref field, MakeParameters(), default);
			}

			return field;
		}
	}

	public override TypeSymbol ReturnType
	{
		get
		{
			if (field == null)
			{
				Interlocked.CompareExchange(ref field, MakeReturnType(), null);
			}

			return field;
		}
	}

	public override ImmutableArray<LifetimeSymbol> Lifetimes => [];
	public override ImmutableArray<LifetimeConstraint> LifetimeConstraints => [];

	public SourceConstructorSymbol(TypeSymbol containingType, BaseConstructorDeclarationSyntax syntax)
	{
		ContainingSymbol = containingType;
		Syntax = syntax;

		if (syntax is NamedConstructorDeclarationSyntax namedCtor)
		{
			Name = namedCtor.Name.GetName();
		}
		else
		{
			Name = string.Empty;
		}
	}

	private ImmutableArray<ParameterSymbol> MakeParameters()
	{
		var builder = ArrayBuilder<ParameterSymbol>.GetInstance();

		TypeSymbol selfType = DeclaringCompilation!.CreateReferenceType(ContainingSymbol, isMutable: true, isNullable: false);
		var selfParam = new SynthesizedParameterSymbol(this, "self", selfType, ordinal: 0);
		builder.Add(selfParam);

		Debug.Assert(Syntax != null);
		int ord = 1;
		foreach (var param in Syntax.ParameterList.Parameters)
		{
			if (param is ParameterSyntax ps)
			{
				var sourceParam = new SourceConstructorParameterSymbol(this, ps, ord);
				builder.Add(sourceParam);
				ord++;
			}
			else if (param is SelfParameterSyntax sps)
			{
				TypeSymbol fieldType = ResolveFieldType(sps.FieldName.GetName());
				var synthParam = new SynthesizedParameterSymbol(this, sps.FieldName.GetName(), fieldType, ord);
				builder.Add(synthParam);
				ord++;
			}
			else
			{
				throw new UnreachableException();
			}
		}

		return builder.ToImmutableAndFree();
	}

	private TypeSymbol ResolveFieldType(string fieldName)
	{
		var result = ContainingSymbol.GetMembers(fieldName);

		if (result.Length == 1 && result[0] is TypeSymbol type)
		{
			return type;
		}

		return new ErrorTypeSymbol(DeclaringCompilation!, SpecialType.None, "<unknown_field>", lifetimeArity: 0, arity: 0, errorInfo: null, unreported: false);
	}

	private TypeSymbol MakeReturnType()
	{
		return DeclaringCompilation!.GetSpecialType(SpecialType.StdVoid);
	}

	public Binder? TryGetInFunctionBinder(BinderFactory? binderFactory = null)
	{
		SyntaxNode inNode = GetInFunctionSyntaxNode();

		Binder result = (binderFactory ?? DeclaringCompilation!.GetBinderFactory(inNode.Tree)).GetBinder(inNode);
#if DEBUG
		Binder? current = result;
		do
		{
			if (current is InFunctionBinder)
			{
				break;
			}

			current = current.Parent;
		}
		while (current != null);

		Debug.Assert(current is InFunctionBinder);
#endif
		return result;
	}

	private FunctionBodySyntax GetInFunctionSyntaxNode()
	{
		return Syntax.Body;
	}

	public Binder? TryGetBodyBinder()
	{
		Binder? inFunctionBinder = TryGetInFunctionBinder();
		FunctionBodySyntax body = GetInFunctionSyntaxNode();
		SyntaxNode? syntax = null;
		if (body is BlockFunctionBodySyntax blockBody)
		{
			syntax = blockBody.Block;
		}

		return inFunctionBinder == null
			? null
			: (syntax == null ? inFunctionBinder : new ExecutableCodeBinder(syntax, this, inFunctionBinder));
	}

	public bool TryBindBody(bool lower, [NotNullWhen(true)] out Binder? binder, [NotNullWhen(true)]
		out BoundFunctionBody? body, BindingDiagnosticBag diagnostics)
	{
		binder = TryGetBodyBinder();
		body = null;

		if (binder == null)
		{
			return false;
		}

		body = (BoundFunctionBody)binder.BindFunctionBody(Syntax, diagnostics);
		Debug.Assert(body != null);
		return true;
	}

	public override string ToDisplayString(SymbolFormat format = SymbolFormat.Default)
	{
		string result = $"{ContainingSymbol.ToDisplayString(format)} operator new";
		result += $"({string.Join(", ", Parameters.Select(t => t.ToDisplayString(format)))})";
		return result;
	}
}
