using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeedMesApp
{
  public partial class MainForm : Form
  {
    private DataGridView dgvMeasurements;
    private Button btnAddMeasurement;
    private Button btnDeleteSelected;
    private Button btnShowGraph;
    private Button btnSave;
    private Button btnLoad;
    private MeasurementInputControl inputControl;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem loadMenuItem;
    private ToolStripMenuItem saveMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private ToolStripMenuItem settingsMenu;
    private ToolStripMenuItem confirmOverwriteMenuItem;

    private List<Measurement> measurements = new List<Measurement>();
    private bool confirmation = true;

    public MainForm()
    {
      this.Text = "Sebességmérő alkalmazás";
      this.Width = 650;
      this.Height = 600;
      this.StartPosition = FormStartPosition.CenterScreen;
      InitializeComponents();
    }

    private void InitializeComponents()
    {
      menuStrip = new MenuStrip();

      // File menu 
      fileMenu = new ToolStripMenuItem("&Fájl");
      loadMenuItem = new ToolStripMenuItem("&Betöltés", null, LoadMenuItem_Click, Keys.Control | Keys.L);
      saveMenuItem = new ToolStripMenuItem("&Mentés", null, SaveMenuItem_Click, Keys.Control | Keys.S);
      exitMenuItem = new ToolStripMenuItem("&Kilépés", null, ExitMenuItem_Click, Keys.Alt | Keys.F4);
      fileMenu.DropDownItems.Add(loadMenuItem);
      fileMenu.DropDownItems.Add(new ToolStripSeparator());
      fileMenu.DropDownItems.Add(exitMenuItem);

      // settings menu
      settingsMenu = new ToolStripMenuItem("&Beállítások");
      confirmOverwriteMenuItem = new ToolStripMenuItem("Megerősítés &felülírás előtt", null, ConfirmOverwriteMenuItem_Click)
      { Checked = confirmation, CheckOnClick = true };
      settingsMenu.DropDownItems.Add(confirmOverwriteMenuItem);

      menuStrip.Items.Add(fileMenu);
      menuStrip.Items.Add(settingsMenu);
      this.MainMenuStrip = menuStrip;
      this.Controls.Add(menuStrip);

      // Measurement 
      inputControl = new MeasurementInputControl()
      {
        Location = new Point(10, menuStrip.Height + 10) // menu after
      };
      this.Controls.Add(inputControl);

      // Measurement add button
      btnAddMeasurement = new Button()
      {
        Text = "Mérés hozzáadása",
        Location = new Point(450, inputControl.Location.Y),
        Width = 150,
        Height = 30,
        BackColor = Color.Purple,
        ForeColor = Color.White,
        Font = new Font("Arial", 8, FontStyle.Bold),
      };
      btnAddMeasurement.Click += BtnAddMeasurement_Click;
      this.Controls.Add(btnAddMeasurement);

      // selected delete func
      btnDeleteSelected = new Button()
      {
        Text = "Kijelölt törlése",
        Location = new Point(450, inputControl.Location.Y + inputControl.Height + 10),
        Width = 150,
        Height = 30,
        BackColor = Color.IndianRed,
        ForeColor = Color.White,
        Font = new Font("Arial", 8, FontStyle.Bold),
      };
      btnDeleteSelected.Click += BtnDeleteSelected_Click;
      this.Controls.Add(btnDeleteSelected);

      // DataGridView dislpay Measurements
      dgvMeasurements = new DataGridView()
      {
        Location = new Point(10, inputControl.Location.Y + inputControl.Height + 10),
        Width = 420,
        Height = 300,
        ReadOnly = true,
        AllowUserToAddRows = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoGenerateColumns = false,
      };
      dgvMeasurements.Columns.Add(new DataGridViewTextBoxColumn()
      {
        Name = "Time",
        HeaderText = "Idő (s)",
        DataPropertyName = "Time",
        Width = this.dgvMeasurements.Width / 2 - 22
      }); // "Time (s)"
      dgvMeasurements.Columns.Add(new DataGridViewTextBoxColumn()
      {
        Name = "Speed",
        HeaderText = "Sebesség (m/s)",
        DataPropertyName = "Speed",
        Width = this.dgvMeasurements.Width / 2 - 22
      }); // "Speed (m/s)"
      this.Controls.Add(dgvMeasurements);

      btnShowGraph = new Button()
      {
        Text = "Grafikon megjelenítése",
        Location = new Point(10, dgvMeasurements.Location.Y + dgvMeasurements.Height + 10),
        Width = 150,
        Height = 30,
        BackColor = Color.BlueViolet,
        ForeColor = Color.White,
        Font = new Font("Arial", 8, FontStyle.Bold),
      };
      btnShowGraph.Click += BtnShowGraph_Click;
      this.Controls.Add(btnShowGraph);

      btnSave = new Button()
      {
        Text = "Mentés",
        Location = new Point(170, dgvMeasurements.Location.Y + dgvMeasurements.Height + 10),
        Width = 150,
        Height = 30,
        BackColor = Color.MediumSeaGreen,
        ForeColor = Color.White,
        Font = new Font("Arial", 8, FontStyle.Bold),
      };
      btnSave.Click += BtnSave_Click;
      this.Controls.Add(btnSave);

      btnLoad = new Button()
      {
        Text = "Betöltés",
        Location = new Point(330, dgvMeasurements.Location.Y + dgvMeasurements.Height + 10),
        Width = 100,
        Height = 30,
        BackColor = Color.LightBlue,
        ForeColor = Color.DarkBlue,
        Font = new Font("Arial", 8, FontStyle.Bold),
      };
      btnLoad.Click += BtnLoad_Click;
      this.Controls.Add(btnLoad);
    }

    /// <summary>
    /// Starts the loading process.
    /// </summary>
    private async void LoadMenuItem_Click(object sender, EventArgs e)
    {
      await LoadAsync(); // loading
    }

    /// <summary>
    /// Starts the save process.
    /// </summary>
    private async void SaveMenuItem_Click(object sender, EventArgs e)
    {
      await SaveAsync();
    }

    /// <summary>
    /// Closes the application.
    /// </summary>
    private void ExitMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Toggles the confirmation option.
    /// </summary>
    private void ConfirmOverwriteMenuItem_Click(object sender, EventArgs e)
    {
      confirmation = confirmOverwriteMenuItem.Checked;
    }

    /// <summary>
    /// Adds a new measurement to the list.
    /// </summary>
    private void BtnAddMeasurement_Click(object sender, EventArgs e)
    {
      if (inputControl.TryGetMeasurement(out Measurement newMeasurement))
      {
        if (measurements.Any(m => m.Time == newMeasurement.Time))
        {
          MessageBox.Show("Duplikált időpont nem megengedett.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        measurements.Add(newMeasurement); // new measurement 
        measurements = measurements.OrderBy(m => m.Time).ToList();
        RefreshDataGrid();
        inputControl.ClearInputs();
      }
      else
      {
        MessageBox.Show("Érvénytelen bevitel. Kérjük, adj meg nem negatív számértékeket az időre és a sebességre.", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    /// <summary>
    /// Deletes the selected measurements from the list.
    /// </summary>
    private void BtnDeleteSelected_Click(object sender, EventArgs e)
    {
      if (dgvMeasurements.SelectedRows.Count > 0)
      {
        foreach (DataGridViewRow row in dgvMeasurements.SelectedRows)
        {
          // Use the column name, not the header text
          double time = Convert.ToDouble(row.Cells["Time"].Value);
          measurements.RemoveAll(m => m.Time == time);
        }
        RefreshDataGrid();
      }
      else
      {
        MessageBox.Show("Kérjük, válasszon ki legalább egy sort a törléshez.", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    /// <summary>
    /// Starts the save process.
    /// </summary>
    private async void BtnSave_Click(object sender, EventArgs e)
    {
      await SaveAsync();
    }

    /// <summary>
    /// Starts the loading process.
    /// </summary>
    private async void BtnLoad_Click(object sender, EventArgs e)
    {
      await LoadAsync();
    }

    /// <summary>
    /// Displays the graph window.
    /// </summary>
    private void BtnShowGraph_Click(object sender, EventArgs e)
    {
      if (measurements.Count < 2) // If less than two measurements
      {
        MessageBox.Show("Legalább két mérés szükséges a grafikon megjelenítéséhez és az út kiszámításához.", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }
      GraphForm graphForm = new GraphForm(measurements);
      graphForm.Show();
    }

    /// <summary>
    /// Updates the DataGridView with the current list of measurements.
    /// </summary>
    private void RefreshDataGrid()
    {
      dgvMeasurements.DataSource = null;
      dgvMeasurements.DataSource = measurements.Select(m => new { m.Time, m.Speed }).ToList();
    }

    /// <summary>
    /// It saves measurements asynchronously to a CSV file.
    /// </summary>
    private async Task SaveAsync()
    {
      if (measurements.Count == 0) // If there are no measurements
      {
        MessageBox.Show("Nincsenek mérések mentésre.", "Információ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV|*.csv", Title = "Mérések mentése" })
      {
        if (sfd.ShowDialog() == DialogResult.OK)
        {
          string filePath = sfd.FileName;
          try
          {
            await Task.Run(() =>
            {
              using (StreamWriter sw = new StreamWriter(filePath))
              {
                sw.WriteLine("Idő,Sebesség");
                foreach (var m in measurements)
                {
                  sw.WriteLine($"{m.Time},{m.Speed}");
                }
              }
            });
            MessageBox.Show("Mérések sikeresen mentve.", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Hiba a mentés során: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    /// <summary>
    /// Load measurements asynchronously from a CSV file.
    /// </summary>
    private async Task LoadAsync()
    {
      using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "CSV|*.csv", Title = "Mérések betöltése" })
      {
        if (ofd.ShowDialog() == DialogResult.OK)
        {
          if (measurements.Count > 0 && confirmation) // If there are already measurements and confirmation is required
          {
            var result = MessageBox.Show("A betöltés felülírja a jelenlegi mérési listát. Folytatod?", "Megerősítés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
              return;
          }

          string filePath = ofd.FileName;
          try
          {
            var loadedMeasurements = await Task.Run(() =>
            {
              var list = new List<Measurement>();
              using (StreamReader sr = new StreamReader(filePath))
              {
                string header = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                  string line = sr.ReadLine();
                  var parts = line.Split(',');
                  if (parts.Length == 2 &&
                      double.TryParse(parts[0], out double time) &&
                      double.TryParse(parts[1], out double speed) &&
                      time >= 0 && speed >= 0)
                  {
                    list.Add(new Measurement(time, speed));
                  }
                }
              }
              return list;
            });

            measurements = loadedMeasurements.OrderBy(m => m.Time).ToList();
            RefreshDataGrid();
            MessageBox.Show("Mérések sikeresen betöltve.", "Siker", MessageBoxButtons.OK, MessageBoxIcon.Information);
          }
          catch (Exception ex)
          {
            MessageBox.Show($"Hiba a betöltés során: {ex.Message}", "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }
  }
}
