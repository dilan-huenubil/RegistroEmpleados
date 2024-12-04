namespace RegistroEmpleados.AppMovil
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Vistas.ListarEmpleados());
        }
    }
}
