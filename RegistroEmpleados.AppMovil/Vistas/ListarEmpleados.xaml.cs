using Firebase.Database;
using System.Collections.ObjectModel;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class ListarEmpleados : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-ce20b-default-rtdb.firebaseio.com/");
    public ObservableCollection<Empleado> Lista { get; set; } = new ObservableCollection<Empleado>();

	public ListarEmpleados()
	{
        InitializeComponent();
        BindingContext = this;
        CargarLista();
	}

    private void CargarLista()
    {
        client.Child("Empleados").AsObservable<Empleado>().Subscribe((empleado) =>
        {
            if (empleado != null)
            {
                Lista.Add(empleado.Object);
            }
        });
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = filtroSearchBar.Text.ToLower();

        if(filtro.Length > 0)
        {
            listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoEmpleadoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearEmpleado());
    }
}