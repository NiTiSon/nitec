namespace Nlr.Tests;

internal static class CodeBase
{
	public const string ExampleAddFunction = """
		module something;
		
		public add(x: i32, y: i32) -> i32 {
			return x + y;
		}
		""";
}