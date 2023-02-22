using System.Runtime.InteropServices;

namespace MicrophoneLevelLogger.Client.View;

public class ConsoleEx
{
    public static int CursorTop => Console.CursorTop;

    public static void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    public static void Wait(TimeSpan timeout)
    {
        var completed = false;
        Task.Delay(timeout).ContinueWith(_ =>
        {
            // 読み込みが未完了の場合だけ中断する。
            // ReSharper disable once AccessToModifiedClosure
            if (!completed)
            {
                var handle = GetStdHandle(StdHandle.Stdin);
                CancelIoEx(handle, IntPtr.Zero);
            }
        });

        try
        {
            Console.ReadLine();
            completed = true;
        }
        catch (InvalidOperationException)
        {
        }
        catch (OperationCanceledException)
        {
        }
    }

    public static void WriteLine(string line = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
    {
        var beforeForeground = Console.ForegroundColor;
        var beforeBackground = Console.BackgroundColor;

        try
        {
            if (foreground is not null) Console.ForegroundColor = (ConsoleColor) foreground;
            if (background is not null) Console.BackgroundColor = (ConsoleColor) background;

            Console.WriteLine(line);
        }
        finally
        {
            Console.ForegroundColor = beforeForeground;
            Console.BackgroundColor = beforeBackground;
        }
    }

    public static void Write(string value = "")
    {
        Console.Write(value);
    }

    private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(StdHandle stdHandle);
    [DllImport("kernel32.dll")]
    private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
}