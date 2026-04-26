using System.Diagnostics;
using System.Runtime.InteropServices;
using LLVMSharp.Interop;

namespace NiteCompiler.Emitting;

internal static class LlvmOptimizer
{
	public static unsafe void Optimize(LLVMModuleRef module, LLVMTargetMachineRef targetMachine)
	{
		LLVMPassBuilderOptionsRef options = LLVMPassBuilderOptionsRef.Create();

		options.SetLoopInterleaving(true);
		options.SetLoopUnrolling(true);
		options.SetLoopVectorization(true);
		options.SetSLPVectorization(true);

		options.SetVerifyEach(true);
		options.SetVerifyEach(true);

		fixed (byte* pOpt = "default<O3>\0"u8)
		{
			LLVMOpaqueError* error = LLVM.RunPasses(module, (sbyte*)pOpt, targetMachine, options);

			if (error != null)
			{
				sbyte* pError = LLVM.GetErrorMessage(error);
				string? message = Marshal.PtrToStringUTF8((nint)pError);
				LLVM.DisposeErrorMessage(pError);
				Debug.WriteLine(message);
			}
		}
	}
}