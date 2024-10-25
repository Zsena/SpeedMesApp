using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SpeedMesApp
{
  public partial class GraphForm : Form
  {
    private List<Measurement> measurements;
    private PictureBox pictureBox;
    private Label lblTotalDistance;

    /// <summary>
    /// Initializes the GraphForm
    /// </summary>
    /// <param name="measurements">List of measurements to be displayed.</param>
    public GraphForm(List<Measurement> measurements)
    {
      this.measurements = measurements;
      this.Text = "Idő - Sebesség és út grafikon";
      this.Width = 800;
      this.Height = 600;
      this.StartPosition = FormStartPosition.CenterParent;
      InitializeComponents();
      DrawGraph();
      CalculateDistance();
    }

    /// <summary>
    /// Configures the GraphForm UI components.
    /// </summary>
    private void InitializeComponents()
    {
      // PictureBox to draw the graph
      pictureBox = new PictureBox()
      {
        Location = new Point(10, 10),
        Width = 760,
        Height = 500,
        BorderStyle = BorderStyle.FixedSingle
      };
      this.Controls.Add(pictureBox);

      // Label to display all traveled routes
      lblTotalDistance = new Label()
      {
        Text = "Teljes megtett út: ",
        Location = new Point(10, 520),
        AutoSize = true,
        Font = new Font("Arial", 12, FontStyle.Bold | FontStyle.Underline)
      };
      this.Controls.Add(lblTotalDistance);
    }

    /// <summary>
    /// Draws the time-latitude graph in the PictureBox.
    /// </summary>
    private void DrawGraph()
    {
      Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
      using (Graphics g = Graphics.FromImage(bmp))
      {
        g.Clear(Color.White);

        int margin = 60; // Increased margin for better positioning of labels
        Rectangle drawingArea = new Rectangle(margin, margin, bmp.Width - 2 * margin, bmp.Height - 2 * margin);

        // Draw X and Y axes
        g.DrawLine(Pens.Black, drawingArea.Left, drawingArea.Bottom, drawingArea.Right, drawingArea.Bottom); // X axis
        g.DrawLine(Pens.Black, drawingArea.Left, drawingArea.Top, drawingArea.Left, drawingArea.Bottom); // Y axis

        double maxTime = measurements.Max(m => m.Time);
        double maxSpeed = measurements.Max(m => m.Speed);

        if (maxTime == 0) maxTime = 1;
        if (maxSpeed == 0) maxSpeed = 1;

        // Draw axis labels
        g.DrawString("Idő (s)", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, drawingArea.Right - 30, drawingArea.Bottom + 40);
        g.DrawString("Sebesség (m/s)", new Font("Arial", 10, FontStyle.Bold), Brushes.Black, drawingArea.Left - 50, drawingArea.Top - 40);

        // Draw grid lines and labels for X and Y axis
        int numXGridLines = 10;
        int numYGridLines = 10;

        for (int i = 0; i <= numXGridLines; i++)
        {
          float x = drawingArea.Left + i * drawingArea.Width / numXGridLines;
          g.DrawLine(Pens.LightGray, x, drawingArea.Top, x, drawingArea.Bottom);
          double timeValue = i * maxTime / numXGridLines;
          g.DrawString(timeValue.ToString("0.0"), new Font("Arial", 8), Brushes.Black, x - 10, drawingArea.Bottom + 5);
        }

        for (int i = 0; i <= numYGridLines; i++)
        {
          float y = drawingArea.Bottom - i * drawingArea.Height / numYGridLines;
          g.DrawLine(Pens.LightGray, drawingArea.Left, y, drawingArea.Right, y);
          double speedValue = i * maxSpeed / numYGridLines;
          g.DrawString(speedValue.ToString("0.0"), new Font("Arial", 8), Brushes.Black, drawingArea.Left - 40, y - 6);
        }

        // Draw lines between measurements if there are at least two
        if (measurements.Count >= 2)
        {
          for (int i = 0; i < measurements.Count - 1; i++)
          {
            var m1 = measurements[i];
            var m2 = measurements[i + 1];

            float x1 = drawingArea.Left + (float)(m1.Time / maxTime) * drawingArea.Width;
            float y1 = drawingArea.Bottom - (float)(m1.Speed / maxSpeed) * drawingArea.Height;
            float x2 = drawingArea.Left + (float)(m2.Time / maxTime) * drawingArea.Width;
            float y2 = drawingArea.Bottom - (float)(m2.Speed / maxSpeed) * drawingArea.Height;
            g.DrawLine(Pens.Blue, x1, y1, x2, y2);
          }
        }

        // Draw measurement points
        foreach (var m in measurements)
        {
          float x = drawingArea.Left + (float)(m.Time / maxTime) * drawingArea.Width;
          float y = drawingArea.Bottom - (float)(m.Speed / maxSpeed) * drawingArea.Height;
          g.FillEllipse(Brushes.Red, x - 3, y - 3, 6, 6);
        }
      }
      pictureBox.Image = bmp;
    }

    /// <summary>
    /// It calculates and displays the total distance traveled based on the measurements.
    /// </summary>
    private void CalculateDistance() 
    {
      double distance = 0.0;
      for (int i = 0; i < measurements.Count - 1; i++)
      {
        double t1 = measurements[i].Time;
        double t2 = measurements[i + 1].Time;
        double v1 = measurements[i].Speed;
        double v2 = measurements[i + 1].Speed;
        double dt = t2 - t1;

        distance += ((v1 + v2) / 2.0) * dt; 
      }
      lblTotalDistance.Text = $"Teljes megtett út: {distance} m";
    }
  }
}
