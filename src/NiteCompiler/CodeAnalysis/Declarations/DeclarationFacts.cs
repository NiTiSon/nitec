using System.Collections.Generic;
using System.ComponentModel;
using NiteCompiler.CodeAnalysis.Syntax;

namespace NiteCompiler.CodeAnalysis.Declarations;

internal static class DeclarationFacts
{
	public static DeclarationModifiers ModifiersFromSyntax(IEnumerable<Token> modifiers)
	{
		DeclarationModifiers result = DeclarationModifiers.None;
		foreach (Token modifier in modifiers)
		{
			result |= ModifierFromSyntaxKind(modifier.Kind);
		}

		return result;
	}

	public static DeclarationModifiers ModifierFromSyntaxKind(SyntaxKind syntaxKind)
	{
		return syntaxKind switch
		{
			SyntaxKind.PublicKeyword => DeclarationModifiers.Public,
			SyntaxKind.ProtectedKeyword => DeclarationModifiers.Protected,
			SyntaxKind.PrivateKeyword => DeclarationModifiers.Private,
			SyntaxKind.FriendKeyword => DeclarationModifiers.Friend,
			SyntaxKind.FamilyKeyword => DeclarationModifiers.Family,
			SyntaxKind.InternalKeyword => DeclarationModifiers.Internal,
			SyntaxKind.AbstractKeyword => DeclarationModifiers.Abstract,
			SyntaxKind.OverrideKeyword => DeclarationModifiers.Override,
			SyntaxKind.SealedKeyword => DeclarationModifiers.Sealed,
			SyntaxKind.VirtualKeyword => DeclarationModifiers.Virtual,
			SyntaxKind.StaticKeyword => DeclarationModifiers.Static,
			SyntaxKind.ConstKeyword => DeclarationModifiers.Const,
			//SyntaxKind.PartialKeyword => DeclarationModifiers.Partial,
			//SyntaxKind.UnsafeKeyword => DeclarationModifiers.Unsafe,
			_ => throw new InvalidEnumArgumentException(nameof(syntaxKind), (int)syntaxKind, typeof(SyntaxKind))
		};
	}

	public static bool IsPartial(this DeclarationModifiers modifiers)
	{
		return modifiers.HasFlag(DeclarationModifiers.Partial);
	}
}