using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class EditarEmpleado : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-ce20b-default-rtdb.firebaseio.com/");
    public List<Cargo> Cargos { get; set; }
    public ObservableCollection<string> ListaCargos { get; set; } = new ObservableCollection<string>();
    private Empleado empleadoActual = new Empleado();
    private string empleadoId;
    public EditarEmpleado(string idEmpleado)
	{
		InitializeComponent();
        BindingContext = this;
        empleadoId = idEmpleado;
        CargarListaCargos();
        CargarEmpleado(empleadoId);
	}

    private async void CargarListaCargos()
    {
        try
        {
            var cargos = await client.Child("Cargos").OnceAsync<Cargo>();
            ListaCargos.Clear();
            foreach (var cargo in cargos)
            {
                ListaCargos.Add(cargo.Object.Nombre);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error:" +ex.Message, "OK");
        }



    }

    private async void CargarEmpleado(string idEmpleado)
    {
        var empleado = await client.Child("Empleados").Child(idEmpleado).OnceSingleAsync<Empleado>();

        if(empleado != null)
        {
            EditPrimerNombreEntry.Text = empleado.PrimerNombre;
            EditSegundoNombreEntry.Text = empleado.SegundoNombre;
            EditPrimerApellidoEntry.Text = empleado.PrimerApellido;
            EditSegundoApellidoEntry.Text = empleado.SegundoApellido;
            EditCorreoEntry.Text = empleado.CorreoElectronico;
            EditSueldoEntry.Text = empleado.Sueldo.ToString();
            EditCargoPicker.SelectedItem = empleado.Cargo?.Nombre;
        }
    }

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(EditPrimerNombreEntry.Text)||
                    string.IsNullOrWhiteSpace(EditSegundoNombreEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditPrimerApellidoEntry.Text)||
                    string.IsNullOrWhiteSpace(EditSegundoApellidoEntry.Text) ||
                    string.IsNullOrWhiteSpace(EditCorreoEntry.Text)||
                    string.IsNullOrWhiteSpace(EditSueldoEntry.Text) ||
                    EditCargoPicker.SelectedItem == null
                    )
            {
                await DisplayAlert("Error", "Todos los Campos son obligatorios", "OK");
                return;
            }

            if (!EditCorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "El correo electr�nico no es v�lido", "OK");
                return;
            }
            
            if(!int.TryParse(EditSueldoEntry.Text, out var sueldo))
            {
                await DisplayAlert("Error", "El sueldo no es un n�mero v�lido", "OK");
                return;
            }

            if(sueldo <= 0)
            {
                await DisplayAlert("Error", "El sueldo debe ser mayor o igual a cero", "OK");
                return;
            }
            empleadoActual.Id = empleadoId;
            empleadoActual.PrimerNombre = EditPrimerNombreEntry.Text.Trim();
            empleadoActual.SegundoNombre = EditSegundoNombreEntry.Text.Trim();
            empleadoActual.PrimerApellido = EditPrimerApellidoEntry.Text.Trim();
            empleadoActual.SegundoApellido = EditSegundoApellidoEntry.Text.Trim();
            empleadoActual.CorreoElectronico = EditCorreoEntry.Text.Trim();
            empleadoActual.Sueldo = sueldo;
            empleadoActual.Cargo = new Cargo { Nombre = EditCargoPicker.SelectedItem.ToString()};
            empleadoActual.Estado = editEstadoSwitch.IsToggled;
             await client.Child("Empleados").Child(empleadoActual.Id).PatchAsync(empleadoActual);
            await DisplayAlert("�xito", "El empleado se ha modificado de manera correcta", "OK");
            await Navigation.PopAsync();
        }
        catch(Exception)
        {
            throw;
        }
    }
}