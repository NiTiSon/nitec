namespace NiTiS.Compiler.NiteCode.CodeAnalysis.Syntax;

public partial class NiteLexer
{
	private void ReadNumberPrefixless()
	{
		ReadNumberX10();
		ReadNumberPostfix();
	}

	private void ReadNumber()
	{
		if (Window.Current == '0')
		{
			switch (Window.Peek(1))
			{
				case 'x':
					ReadNumberX16();
					break;
				case 'b':
					ReadNumberX2();
					break;
				default:
					ReadNumberX10();
					break;
			}
		}

		ReadNumberPostfix();
	}

	private void ReadNumberX10()
	{
		if (char.IsAsciiDigit(Window.Current))
		{
			int len = 1;
			char c = Window.Peek(len);
			while (char.IsAsciiDigit(c) || c == '\'')
			{
				len++;
				c = Window.Peek(len);
			}
			Window.Advance(len);
		}
	}

	private void ReadNumberX16()
	{
		if (char.IsAsciiHexDigit(Window.Current))
		{
			int len = 1;
			char c = Window.Peek(len);
			while (char.IsAsciiHexDigit(c) || c == '\'')
			{
				len++;
				c = Window.Peek(len);
			}
			Window.Advance(len);
		}
	}

	private void ReadNumberX2()
	{
		if (Window.Current is '0' or '1')
		{
			int len = 1;
			char c = Window.Peek(len);
			while (c is '0' or '1' or '\'')
			{
				len++;
				c = Window.Peek(len);
			}
			Window.Advance(len);
		}
	}

	private bool ReadNumberPostfix()
	{
		char current = Window.Current;
		if (current is 'i' or 'u' or 'f')
		{
			char next = Window.Next;
			switch (next)
			{
				case '8' when current is not 'f':
					Window.Advance(2);
					break;
				case '1' when Window.Peek(2) == '6':
				case '3' when Window.Peek(2) == '2':
				case '6' when Window.Peek(2) == '4':
					Window.Advance(3);
					break;
			}

			return true;
		}
		return false;
	}
}