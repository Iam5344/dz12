using System;
using System.Runtime.InteropServices;

class NativeMethods
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("kernel32.dll")]
    public static extern bool Beep(uint dwFreq, uint dwDuration);

    [DllImport("user32.dll")]
    public static extern bool MessageBeep(uint uType);
}

class Program
{
    static void Main()
    {
        NativeMethods.MessageBox(IntPtr.Zero, "Ім'я: Глеб", "Інформація про мене", 0);
        NativeMethods.MessageBox(IntPtr.Zero, "Вік: 18", "Інформація про мене", 0);
        NativeMethods.MessageBox(IntPtr.Zero, "Спеціальність: Програмування", "Інформація про мене", 0);

        NativeMethods.Beep(500, 300);
        System.Threading.Thread.Sleep(500);
        NativeMethods.Beep(1000, 300);
        System.Threading.Thread.Sleep(500);

        NativeMethods.Beep(1500, 300);
        System.Threading.Thread.Sleep(500);

        NativeMethods.Beep(2000, 300);
        System.Threading.Thread.Sleep(500);

        NativeMethods.Beep(2500, 300);
        System.Threading.Thread.Sleep(500);

        NativeMethods.MessageBeep(0x00000000);
        System.Threading.Thread.Sleep(500);

        NativeMethods.MessageBeep(0x00000010);
        System.Threading.Thread.Sleep(500);

        NativeMethods.MessageBeep(0x00000020);
        System.Threading.Thread.Sleep(500);

        NativeMethods.MessageBeep(0x00000030);
        System.Threading.Thread.Sleep(500);

        NativeMethods.MessageBeep(0x00000040);
    }
}
