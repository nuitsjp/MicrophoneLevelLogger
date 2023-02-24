using System.Runtime.InteropServices;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// 拡張コンソール
/// </summary>
public class ConsoleEx
{
    public static int CursorTop => Console.CursorTop;

    /// <summary>
    /// カーソル位置を移動する。
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    public static void SetCursorPosition(int left, int top)
    {
        Console.SetCursorPosition(left, top);
    }

    /// <summary>
    /// Enterでキャンセル可能な状態で、指定時間待機する。
    /// </summary>
    /// <param name="timeout"></param>
    public static void Wait(TimeSpan timeout)
    {
        var completed = false;
        Task.Delay(timeout).ContinueWith(_ =>
        {
            // 読み込みが未完了の場合だけ中断する。
            // ReSharper disable once AccessToModifiedClosure
            if (!completed)
            {
                // Enterが先に押されていた場合は、処理しない
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

    /// <summary>
    /// 前面・背面の色を指定してラインを表示する。
    /// </summary>
    /// <param name="line"></param>
    /// <param name="foreground"></param>
    /// <param name="background"></param>
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

    /// <summary>
    /// ラインを表示する。
    /// </summary>
    /// <param name="value"></param>
    public static void Write(string value = "")
    {
        Console.Write(value);
    }

    private enum StdHandle { Stdin = -10 };
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(StdHandle stdHandle);
    [DllImport("kernel32.dll")]
    private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);
}