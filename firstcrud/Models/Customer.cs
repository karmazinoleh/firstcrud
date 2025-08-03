using System.Text.Json.Serialization;

namespace firstcrud;

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [JsonIgnore]
    public List<Product> Products { get; set; }
}