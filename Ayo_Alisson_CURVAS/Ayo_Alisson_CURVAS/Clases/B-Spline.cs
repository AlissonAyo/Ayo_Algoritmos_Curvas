using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ayo_Alisson_CURVAS.Calses
{
    /// <summary>
    /// Implementación de curvas B-Spline utilizando el algoritmo de Cox-de Boor.
    /// Las B-Splines son curvas paramétricas definidas por puntos de control y funciones base polinomiales.
    /// A diferencia de las curvas de Bézier, las B-Splines ofrecen control local: modificar un punto
    /// de control solo afecta una porción de la curva, no toda la curva.
    /// </summary>
    internal class B_Spline
    {
        private List<PointF> puntosControl;
        private int grado;

        /// <summary>
        /// Constructor que inicializa una curva B-Spline con un grado específico.
        /// </summary>
        /// <param name="grado">Grado de la curva B-Spline (por defecto 3, que genera curvas cúbicas)</param>
        /// <exception cref="ArgumentException">Si el grado es menor o igual a 0</exception>
        public B_Spline(int grado = 3)
        {
            if (grado <= 0)
            {
                throw new ArgumentException("El grado debe ser mayor que 0.", nameof(grado));
            }

            puntosControl = new List<PointF>();
            this.grado = grado;
        }

        /// <summary>
        /// Agrega un punto de control a la curva B-Spline.
        /// </summary>
        /// <param name="punto">Punto a agregar como punto de control</param>
        /// <exception cref="ArgumentException">Si el punto contiene valores inválidos</exception>
        public void AgregarPuntoControl(PointF punto)
        {
            // Validación: verificar que el punto no contenga valores inválidos
            if (float.IsNaN(punto.X) || float.IsNaN(punto.Y) || 
                float.IsInfinity(punto.X) || float.IsInfinity(punto.Y))
            {
                throw new ArgumentException("El punto contiene valores inválidos (NaN o infinito).");
            }

            puntosControl.Add(punto);
        }

        /// <summary>
        /// Limpia todos los puntos de control de la curva.
        /// </summary>
        public void LimpiarPuntos()
        {
            puntosControl.Clear();
        }

        /// <summary>
        /// Obtiene una copia de la lista de puntos de control.
        /// </summary>
        /// <returns>Lista con los puntos de control actuales</returns>
        public List<PointF> ObtenerPuntosControl()
        {
            return new List<PointF>(puntosControl);
        }

        /// <summary>
        /// Obtiene la cantidad de puntos de control actuales.
        /// </summary>
        /// <returns>Número de puntos de control</returns>
        public int CantidadPuntos()
        {
            return puntosControl.Count;
        }

        /// <summary>
        /// Obtiene el grado actual de la curva B-Spline.
        /// </summary>
        /// <returns>Grado de la curva</returns>
        public int ObtenerGrado()
        {
            return grado;
        }

        /// <summary>
        /// Establece un nuevo grado para la curva B-Spline.
        /// </summary>
        /// <param name="nuevoGrado">Nuevo grado de la curva</param>
        /// <exception cref="ArgumentException">Si el grado es menor o igual a 0</exception>
        public void EstablecerGrado(int nuevoGrado)
        {
            if (nuevoGrado <= 0)
            {
                throw new ArgumentException("El grado debe ser mayor que 0.", nameof(nuevoGrado));
            }
            
            grado = nuevoGrado;
        }

        /// <summary>
        /// Elimina el último punto de control agregado.
        /// </summary>
        /// <returns>True si se eliminó un punto, false si no hay puntos</returns>
        public bool EliminarUltimoPunto()
        {
            if (puntosControl.Count > 0)
            {
                puntosControl.RemoveAt(puntosControl.Count - 1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Genera un vector de nudos (knot vector) uniforme para la curva B-Spline.
        /// El vector de nudos determina cómo se comporta la curva en sus extremos y en el medio.
        /// Se usa un vector uniforme abierto (clamped) que hace que la curva pase por el primer
        /// y último punto de control.
        /// </summary>
        /// <returns>Lista de valores que forman el vector de nudos</returns>
        private List<float> GenerarVectorNudos()
        {
            List<float> nudos = new List<float>();
            int n = puntosControl.Count - 1; // Índice del último punto de control
            int m = n + grado + 1; // Número total de nudos

            // Generar vector de nudos uniforme abierto (clamped uniform)
            for (int i = 0; i <= m; i++)
            {
                if (i <= grado)
                {
                    // Repetir el primer nudo 'grado+1' veces
                    nudos.Add(0);
                }
                else if (i >= n + 1)
                {
                    // Repetir el último nudo 'grado+1' veces
                    nudos.Add(1);
                }
                else
                {
                    // Nudos intermedios distribuidos uniformemente
                    nudos.Add((float)(i - grado) / (n - grado + 1));
                }
            }

            return nudos;
        }

        /// <summary>
        /// Calcula la función base B-Spline N(i,k) usando el algoritmo recursivo de Cox-de Boor.
        /// Las funciones base determinan la influencia de cada punto de control sobre la curva.
        /// </summary>
        /// <param name="i">Índice de la función base</param>
        /// <param name="k">Orden de la función base (grado + 1)</param>
        /// <param name="t">Parámetro de evaluación</param>
        /// <param name="nudos">Vector de nudos</param>
        /// <returns>Valor de la función base en el parámetro t</returns>
        private float FuncionBase(int i, int k, float t, List<float> nudos)
        {
            // Caso base: función base de orden 0 (función constante por tramos)
            if (k == 0)
            {
                // N(i,0) = 1 si t está en [u_i, u_{i+1}), 0 en otro caso
                return (t >= nudos[i] && t < nudos[i + 1]) ? 1.0f : 0.0f;
            }

            // Caso recursivo: aplicar la fórmula de Cox-de Boor
            // N(i,k) = ((t - u_i) / (u_{i+k} - u_i)) * N(i,k-1) + ((u_{i+k+1} - t) / (u_{i+k+1} - u_{i+1})) * N(i+1,k-1)

            float denomA = nudos[i + k] - nudos[i];
            float denomB = nudos[i + k + 1] - nudos[i + 1];

            float termA = 0;
            if (denomA != 0)
            {
                // Primer término de la recursión
                termA = ((t - nudos[i]) / denomA) * FuncionBase(i, k - 1, t, nudos);
            }

            float termB = 0;
            if (denomB != 0)
            {
                // Segundo término de la recursión
                termB = ((nudos[i + k + 1] - t) / denomB) * FuncionBase(i + 1, k - 1, t, nudos);
            }

            return termA + termB;
        }

        /// <summary>
        /// Calcula un punto en la curva B-Spline para un parámetro t dado.
        /// </summary>
        /// <param name="t">Parámetro de la curva (0 ≤ t ≤ 1)</param>
        /// <returns>Punto calculado en la curva</returns>
        /// <exception cref="ArgumentOutOfRangeException">Si t está fuera del rango [0, 1]</exception>
        /// <exception cref="InvalidOperationException">Si no hay suficientes puntos de control</exception>
        public PointF CalcularPunto(float t)
        {
            // Validación: verificar que t esté en el rango válido
            if (t < 0.0f || t > 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(t), 
                    "El parámetro t debe estar en el rango [0, 1].");
            }

            // Validación: verificar que haya suficientes puntos de control
            if (puntosControl.Count <= grado)
            {
                throw new InvalidOperationException(
                    $"Se necesitan al menos {grado + 1} puntos de control para una curva de grado {grado}.");
            }

            List<float> nudos = GenerarVectorNudos();
            
            // Ajustar t para evitar problemas en el extremo (t = 1.0)
            if (t >= 1.0f)
                t = 0.9999f;

            float x = 0;
            float y = 0;

            // Calcular el punto como combinación lineal de puntos de control
            // ponderados por las funciones base B-Spline
            // P(t) = Σ N(i,k)(t) * P_i
            for (int i = 0; i < puntosControl.Count; i++)
            {
                float base_val = FuncionBase(i, grado, t, nudos);
                x += puntosControl[i].X * base_val;
                y += puntosControl[i].Y * base_val;
            }

            return new PointF(x, y);
        }

        /// <summary>
        /// Genera la curva B-Spline completa como una secuencia de puntos discretos.
        /// </summary>
        /// <param name="segmentos">Número de segmentos para discretizar la curva</param>
        /// <returns>Lista de puntos que forman la curva</returns>
        /// <exception cref="ArgumentException">Si el número de segmentos es inválido</exception>
        public List<PointF> GenerarCurva(int segmentos = 100)
        {
            List<PointF> curva = new List<PointF>();

            // Validación: verificar número de segmentos válido
            if (segmentos <= 0)
            {
                throw new ArgumentException("El número de segmentos debe ser mayor que 0.", nameof(segmentos));
            }

            // Validación: verificar que haya suficientes puntos de control
            if (puntosControl.Count <= grado)
                return curva;

            try
            {
                // Generar puntos equidistantes a lo largo del parámetro t
                for (int i = 0; i <= segmentos; i++)
                {
                    float t = i / (float)segmentos;
                    PointF punto = CalcularPunto(t);
                    
                    // Validar que el punto calculado sea válido
                    if (!punto.IsEmpty && !float.IsNaN(punto.X) && !float.IsNaN(punto.Y))
                    {
                        curva.Add(punto);
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error durante la generación de la curva
                throw new InvalidOperationException(
                    "Error al generar la curva B-Spline.", ex);
            }

            return curva;
        }

        /// <summary>
        /// Valida si la configuración actual permite generar una curva válida.
        /// </summary>
        /// <returns>True si se puede generar una curva válida</returns>
        public bool EsConfiguracionValida()
        {
            return puntosControl.Count > grado;
        }

        /// <summary>
        /// Obtiene información descriptiva sobre la curva actual.
        /// </summary>
        /// <returns>Cadena con información sobre la curva</returns>
        public string ObtenerInformacion()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine("=== INFORMACIÓN DE CURVA B-SPLINE ===");
            info.AppendLine($"Puntos de control: {puntosControl.Count}");
            info.AppendLine($"Grado de la curva: {grado}");
            info.AppendLine($"Orden de la curva: {grado + 1}");
            info.AppendLine($"Puntos mínimos requeridos: {grado + 1}");
            info.AppendLine($"¿Válida para dibujar?: {(EsConfiguracionValida() ? "Sí" : "No")}");
            
            if (puntosControl.Count > 0)
            {
                info.AppendLine("\nPuntos de control:");
                for (int i = 0; i < puntosControl.Count; i++)
                {
                    info.AppendLine($"  P{i}: ({puntosControl[i].X:F2}, {puntosControl[i].Y:F2})");
                }
            }

            if (EsConfiguracionValida())
            {
                List<float> nudos = GenerarVectorNudos();
                info.AppendLine("\nVector de nudos:");
                info.Append("  [");
                for (int i = 0; i < nudos.Count; i++)
                {
                    info.Append($"{nudos[i]:F3}");
                    if (i < nudos.Count - 1) info.Append(", ");
                }
                info.AppendLine("]");
            }
            
            return info.ToString();
        }
    }
}
