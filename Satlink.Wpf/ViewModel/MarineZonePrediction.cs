namespace Satlink
{
    public class MarineZonePredictionItem : ObservableObject
    {
        private int id;
        private string texto;
        private string nombre;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged("Id");
            }
        }

        public string Texto
        {
            get
            {
                return texto;
            }
            set
            {
                texto = value;
                RaisePropertyChanged("Texto");
            }
        }

        public string Nombre
        {
            get
            {
                return nombre;
            }
            set
            {
                nombre = value;
                RaisePropertyChanged("Nombre");
            }
        }
    }
}
