using static System.AttributeTargets;

namespace NiteCompiler;

[System.AttributeUsage(Module | Class | Struct | Enum | Constructor |
                Method | Property | Field | Event | Interface | Parameter |
                Delegate | ReturnValue | GenericParameter)]
public sealed class ForRemovalAttribute : System.Attribute
{
	public string Message { get; }

	public ForRemovalAttribute(string message)
	{
		Message = message;
	}
}