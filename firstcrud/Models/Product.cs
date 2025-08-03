namespace firstcrud;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int PublisherId { get; set; }
    public Publisher Publisher { get; set; }
    public ProductDetails?  ProductDetails { get; set; }
    public List<Customer>? Customers { get; set; }
}