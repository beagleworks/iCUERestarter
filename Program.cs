namespace iCUERestarter;

static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main()
    {
        const string mutexName = "iCUERestarter_SingleInstance";

        _mutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            MessageBox.Show("iCUE Restarter は既に実行中です。", "iCUE Restarter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplicationContext());
        }
        finally
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
        }
    }
}
