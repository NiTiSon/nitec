using System.Runtime.InteropServices;

namespace Nlr.Compiler;

public sealed class DynamicLinkData
{
	private readonly string _importPath;
	private readonly string _entryPoint;
	private readonly CallingConvention _callConvention;

	public string ImportPath => _importPath;
	public string EntryPoint => _entryPoint;
	public CallingConvention CallConvention => _callConvention;

	public DynamicLinkData(string importPath, string entryPoint, [Optional] CallingConvention callConvention)
	{
		_callConvention = callConvention;
		_importPath = importPath;
		_entryPoint = entryPoint;
	}
}

public enum CallingConvention
{
	Default = 0,
}