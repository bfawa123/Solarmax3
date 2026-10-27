using System;

public class ChessItem
{
	public int id { get; set; }

	public long timeout { get; set; }

	public DateTime timefinish { get; set; }

	public int slot { get; set; }

	public static int CHESS_MAX = 5;
}
