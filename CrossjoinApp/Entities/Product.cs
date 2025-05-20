public class Product
{
    public required string ProductID { get; init; }
    public Product? DependentProduct { get; set; }
    public required ProductType ProductType { get; set; } = ProductType.None;

}