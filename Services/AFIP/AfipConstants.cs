namespace FiyiPOS.Services.AFIP
{
    public static class AfipConstants
    {
        public const string ServiceWsfe = "wsfe";
        public const string AmbienteHomologacion = "Homologacion";
        public const string AmbienteProduccion = "Produccion";

        public const int FacturaA = 1;
        public const int FacturaB = 6;
        public const int FacturaC = 11;

        public const int DocTipoCuit = 80;
        public const int DocTipoDni = 96;
        public const int DocTipoConsumidorFinal = 99;

        public const int Iva21 = 5;
        public const int Iva105 = 4;
        public const int Iva27 = 6;
        public const int Iva0 = 3;

        public static string WsaaUrl(string ambiente) => ambiente == AmbienteProduccion
            ? "https://wsaa.afip.gov.ar/ws/services/LoginCms"
            : "https://wsaahomo.afip.gov.ar/ws/services/LoginCms";

        public static string WsfeUrl(string ambiente) => ambiente == AmbienteProduccion
            ? "https://servicios1.afip.gov.ar/wsfev1/service.asmx"
            : "https://wswhomo.afip.gov.ar/wsfev1/service.asmx";

        public static int MapIvaId(decimal rate) => rate switch
        {
            >= 26m => Iva27,
            >= 20m => Iva21,
            >= 10m => Iva105,
            _ => Iva0
        };
    }
}
