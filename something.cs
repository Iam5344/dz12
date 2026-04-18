using System.Diagnostics;

namespace WinFormsApp5;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        LoadProcesses();
    }

    private void LoadProcesses()
    {
        listBox1.Items.Clear();
        foreach (Process p in Process.GetProcesses())
            listBox1.Items.Add(p.ProcessName + " | PID: " + p.Id);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        LoadProcesses();
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
        timer1.Interval = (int)(numericUpDown1.Value * 1000);
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBox1.SelectedItem == null) return;

        string selected = listBox1.SelectedItem.ToString();
        int pid = int.Parse(selected.Split("PID: ")[1]);

        Process p = Process.GetProcessById(pid);
        p.Refresh();

        label1.Text = "PID: " + p.Id;
        label2.Text = "Потоків: " + p.Threads.Count;
        label3.Text = "Копій: " + Process.GetProcessesByName(p.ProcessName).Length;

        try { label4.Text = "Старт: " + p.StartTime.ToString("HH:mm:ss"); }
        catch { label4.Text = "Старт: недоступно"; }

        try { label5.Text = "CPU: " + p.TotalProcessorTime.ToString(@"hh\:mm\:ss"); }
        catch { label5.Text = "CPU: недоступно"; }

        button1.Enabled = true;
        button1.Tag = pid;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        int pid = (int)button1.Tag;
        if (MessageBox.Show("Завершити процес PID " + pid + "?", "Підтвердження",
            MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            Process.GetProcessById(pid).Kill();
            LoadProcesses();
        }
    }
}
