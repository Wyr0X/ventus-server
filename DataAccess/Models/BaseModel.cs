using System;
using System.Reflection;

public abstract class BaseModel
{
    // Método genérico para imprimir los detalles usando reflexión
    public virtual void PrintInfo()
    {
        Console.WriteLine($"### {this.GetType().Name} Info ###");
        
        // Obtiene todas las propiedades publicas del tipo de la clase que hereda
        var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Obtiene el valor de la propiedad
            var value = property.GetValue(this);
            // Imprime el nombre de la propiedad y su valor
            Console.WriteLine($"{property.Name}: {value}");
        }
        
        Console.WriteLine($"### End of {this.GetType().Name} Info ###");
    }
}
