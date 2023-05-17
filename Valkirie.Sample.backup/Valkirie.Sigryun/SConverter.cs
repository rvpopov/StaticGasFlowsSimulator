namespace Valkyrie.Sigryun
{
    public static class SConverter
    {
        /// <summary>
        /// Конвертирует [Ата] в [МПа]
        /// </summary>
        /// <param name="val">Значение в [Ата]</param>
        /// <returns>Значение в [МПа]</returns>
        public static double AtaToMPa(double val)
        {
            //return val;
            return val * 0.098067;
        }

        /// <summary>
        /// Конвертирует [Ата] в [МПа]
        /// </summary>
        /// <param name="val">Значение в [Ата]</param>
        /// <returns>Значение в [МПа]</returns>
        public static double AtiToMPa(double val)
        {
            //return val;
            return (val + 1.0332) * 0.098067;
        }

        /// <summary>
        /// Конвертирует [МПа] в [Ата]
        /// </summary>
        /// <param name="val">Значение в [МПа]</param>
        /// <returns>Значение в [Ата]</returns>
        public static double MPaToAta(double val)
        {
            //return val;
            return val * 10.197081;
        }

        /// <summary>
        /// Конвертирует [°С] в [°К]
        /// </summary>
        /// <param name="val">Значение в [°С]</param>
        /// <returns>Значение в [°К]</returns>
        public static double CToK(double val)
        {
            return val + 273.15;
        }

        /// <summary>
        /// Конвертирует [°К] в [°С]
        /// </summary>
        /// <param name="val">Значение в [°К]</param>
        /// <returns>Значение в [°С]</returns>
        public static double КToС(double val)
        {
            return val - 273.15;
        }
    }
}
