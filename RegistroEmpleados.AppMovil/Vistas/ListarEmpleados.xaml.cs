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

    private async void CargarLista()
    {
        var empleados = await client.Child("Empleados").OnceAsync<Empleado>();

        var empleadosActivos = empleados.Where(e => e.Object.Estado == true).ToList();

        foreach (var empleado in empleadosActivos)
        {
            Lista.Add(new Empleado
            {
                Id = empleado.Key,
                PrimerNombre = empleado.Object.PrimerNombre,
                SegundoNombre = empleado.Object.SegundoNombre,
                PrimerApellido = empleado.Object.PrimerApellido,
                SegundoApellido = empleado.Object.SegundoApellido,
                CorreoElectronico = empleado.Object.CorreoElectronico,
                Sueldo = empleado.Object.Sueldo,
                FechaInicio = empleado.Object.FechaInicio,
                Estado = empleado.Object.Estado,
                Cargo = empleado.Object.Cargo
            });
        }
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

    private async void EditarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var empleado = boton?.CommandParameter as Empleado;

        if (empleado != null && !string.IsNullOrEmpty(empleado.Id))
        {
            await Navigation.PushAsync(new EditarEmpleado(empleado.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se puedo obtener la información del empleado", "OK");
        }
    }

    private void deshabilitarButton_Clicked(object sender, EventArgs e)
    {

    }
}