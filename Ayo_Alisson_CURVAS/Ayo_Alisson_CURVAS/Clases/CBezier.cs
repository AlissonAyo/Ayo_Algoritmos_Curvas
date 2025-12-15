using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ayo_Alisson_CURVAS.Clases
{
    /// <summary>
    /// Implementación de curvas de Bézier utilizando el algoritmo de De Casteljau.
    /// Una curva de Bézier es una curva paramétrica definida por puntos de control.
    /// El algoritmo de De Casteljau permite evaluar la curva mediante interpolación lineal recursiva.
    /// </summary>
    internal class CBezier
    {
        private List<PointF> puntosControl;

        /// <summary>
        /// Constructor que inicializa la lista de puntos de control vacía.
        /// </summary>
        public CBezier()
        {
            puntosControl = new List<PointF>();
        }

        /// <summary>
        /// Agrega un punto de control a la curva de Bézier.
        /// </summary>
        /// <param name="punto">Punto a agregar como punto de control</param>
        /// <exception cref="ArgumentException">Si el punto contiene valores inválidos (NaN o infinito)</exception>
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
        /// Elimina el último punto de control agregado.
        /// </summary>
        /// <returns>True si se eliminó un punto, false si no hay puntos para eliminar</returns>
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
        /// Algoritmo de De Casteljau para calcular un punto en la curva de Bézier.
        /// Este algoritmo realiza interpolaciones lineales recursivas entre los puntos de control
        /// hasta obtener un único punto que pertenece a la curva.
        /// </summary>
        /// <param name="t">Parámetro de la curva (0 ≤ t ≤ 1)</param>
        /// <returns>Punto calculado en la curva para el parámetro t</returns>
        /// <exception cref="ArgumentOutOfRangeException">Si t está fuera del rango [0, 1]</exception>
        public PointF CalcularPunto(float t)
        {
            // Validación: verificar que t esté en el rango válido
            if (t < 0.0f || t > 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(t), 
                    "El parámetro t debe estar en el rango [0, 1].");
            }

            // Validación: verificar que haya al menos un punto de control
            if (puntosControl.Count == 0)
                return PointF.Empty;

            // Si solo hay un punto, retornarlo directamente
            if (puntosControl.Count == 1)
                return puntosControl[0];

            // Copiar los puntos de control originales
            List<PointF> puntos = new List<PointF>(puntosControl);

            // Algoritmo de De Casteljau: interpolación lineal recursiva
            // En cada iteración, se reduce el número de puntos en 1
            while (puntos.Count > 1)
            {
                List<PointF> nuevosPuntos = new List<PointF>();
                
                // Calcular interpolaciones lineales entre puntos consecutivos
                for (int i = 0; i < puntos.Count - 1; i++)
                {
                    // Fórmula de interpolación lineal: P(t) = (1-t)*P0 + t*P1
                    float x = (1 - t) * puntos[i].X + t * puntos[i + 1].X;
                    float y = (1 - t) * puntos[i].Y + t * puntos[i + 1].Y;
                    nuevosPuntos.Add(new PointF(x, y));
                }
                
                // Los nuevos puntos se convierten en la base para la siguiente iteración
                puntos = nuevosPuntos;
            }

            // El último punto restante es el punto en la curva
            return puntos[0];
        }

        /// <summary>
        /// Genera la curva de Bézier completa como una secuencia de puntos discretos.
        /// </summary>
        /// <param name="segmentos">Número de segmentos para discretizar la curva (mayor = más suave)</param>
        /// <returns>Lista de puntos que forman la curva</returns>
        /// <exception cref="ArgumentException">Si el número de segmentos es menor o igual a 0</exception>
        public List<PointF> GenerarCurva(int segmentos = 100)
        {
            List<PointF> curva = new List<PointF>();

            // Validación: verificar número de segmentos válido
            if (segmentos <= 0)
            {
                throw new ArgumentException("El número de segmentos debe ser mayor que 0.", nameof(segmentos));
            }

            // Validación: se necesitan al menos 2 puntos para generar una curva
            if (puntosControl.Count < 2)
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
                    "Error al generar la curva de Bézier.", ex);
            }

            return curva;
        }

        /// <summary>
        /// Valida si la configuración actual permite generar una curva válida.
        /// </summary>
        /// <returns>True si se puede generar una curva válida, false en caso contrario</returns>
        public bool EsConfiguracionValida()
        {
            return puntosControl.Count >= 2;
        }

        /// <summary>
        /// Obtiene información descriptiva sobre la curva actual.
        /// </summary>
        /// <returns>Cadena con información sobre la curva</returns>
        public string ObtenerInformacion()
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine("=== INFORMACIÓN DE CURVA DE BÉZIER ===");
            info.AppendLine($"Puntos de control: {puntosControl.Count}");
            info.AppendLine($"Grado de la curva: {puntosControl.Count - 1}");
            info.AppendLine($"¿Válida para dibujar?: {(EsConfiguracionValida() ? "Sí" : "No")}");
            
            if (puntosControl.Count > 0)
            {
                info.AppendLine("\nPuntos de control:");
                for (int i = 0; i < puntosControl.Count; i++)
                {
                    info.AppendLine($"  P{i}: ({puntosControl[i].X:F2}, {puntosControl[i].Y:F2})");
                }
            }
            
            return info.ToString();
        }
    }
}
