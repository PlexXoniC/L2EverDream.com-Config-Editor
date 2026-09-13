using System.Diagnostics;
using System.Net.Sockets;

namespace L2Config.Core.Storage;

/// <summary>Read-only checks for whether the world or the game client is running. Starts and stops nothing.</summary>
public static class RuntimeStatus
{
	public static async Task<bool> IsWorldRunningAsync(int gameServerPort, CancellationToken cancellation = default)
	{
		using var client = new TcpClient();
		using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
		timeout.CancelAfter(TimeSpan.FromMilliseconds(400));
		try
		{
			await client.ConnectAsync("127.0.0.1", gameServerPort, timeout.Token);
			return true;
		}
		catch (Exception ex) when (ex is SocketException or OperationCanceledException)
		{
			return false;
		}
	}

	public static bool IsClientRunning()
	{
		var processes = Process.GetProcessesByName("L2");
		foreach (var process in processes)
		{
			process.Dispose();
		}
		return processes.Length > 0;
	}
}
