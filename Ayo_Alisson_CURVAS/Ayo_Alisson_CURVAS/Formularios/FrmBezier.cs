using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ayo_Alisson_CURVAS.Clases;

namespace Ayo_Alisson_CURVAS.Formularios
{
    public partial class FrmBezier : Form
    {
        private CBezier curva;
        private Bitmap lienzo;
        private Graphics graficos;

        public FrmBezier()
        {
            InitializeComponent();
            curva = new CBezier();
            InicializarLienzo();
        }

        private void InicializarLienzo()
        {
            lienzo = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graficos = Graphics.FromImage(lienzo);
            graficos.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graficos.Clear(Color.White);
            pictureBox1.Image = lienzo;
        }

        private void btnAgregarPunto_Click(object sender, EventArgs e)
        {
            try
            {
                float x = float.Parse(txtX.Text);
                float y = float.Parse(txtY.Text);

                curva.AgregarPuntoControl(new PointF(x, y));
                ActualizarListaPuntos();
                DibujarTodo();

                txtX.Clear();
                txtY.Clear();
                txtX.Focus();
            }
            catch (Exception)
            {
                MessageBox.Show("Por favor ingrese valores numéricos válidos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            curva.LimpiarPuntos();
            ActualizarListaPuntos();
            graficos.Clear(Color.White);
            pictureBox1.Refresh();
        }

        private void ActualizarListaPuntos()
        {
            listBoxPuntos.Items.Clear();
            foreach (var punto in curva.ObtenerPuntosControl())
            {
                listBoxPuntos.Items.Add($"({punto.X:F1}, {punto.Y:F1})");
            }
            lblCantidadPuntos.Text = $"Puntos: {curva.CantidadPuntos()}";
        }

        private void DibujarTodo()
        {
            graficos.Clear(Color.White);

            var puntos = curva.ObtenerPuntosControl();

            // Dibujar líneas de control
            if (puntos.Count > 1)
            {
                using (Pen pen = new Pen(Color.LightGray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    for (int i = 0; i < puntos.Count - 1; i++)
                    {
                        graficos.DrawLine(pen, puntos[i], puntos[i + 1]);
                    }
                }
            }

            // Dibujar puntos de control
            foreach (var punto in puntos)
            {
                graficos.FillEllipse(Brushes.Blue, punto.X - 4, punto.Y - 4, 8, 8);
            }

            // Dibujar la curva de Bézier
            if (puntos.Count >= 2)
            {
                var curvaPuntos = curva.GenerarCurva(100);
                if (curvaPuntos.Count > 1)
                {
                    using (Pen pen = new Pen(Color.Red, 2))
                    {
                        graficos.DrawLines(pen, curvaPuntos.ToArray());
                    }
                }
            }

            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            curva.AgregarPuntoControl(new PointF(e.X, e.Y));
            ActualizarListaPuntos();
            DibujarTodo();
        }

        private void FrmBezier_Resize(object sender, EventArgs e)
        {
            if (pictureBox1 != null && pictureBox1.Width > 0 && pictureBox1.Height > 0)
            {
                InicializarLienzo();
                DibujarTodo();
            }
        }

        private void FrmBezier_Load(object sender, EventArgs e)
        {

        }
    }
}
