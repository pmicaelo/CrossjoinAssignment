public class Proposal
{
    public required string ProposalID { get; init; }
    public required Lead Lead { get; init; }
    public Dictionary<string, Product> Products { get; set; } = new();
    public double? ProductionCost { get; set; }
    public int? MonthlyProducedProducts { get; set; }
    public double? ExpectedMonthlyProfit { get; set; }
    public Status Status { get; set; } = Status.Draft;
    public string? Country => Lead.Country;
    public BusinessType BusinessType => Lead.BusinessType;
    public Company Company => Lead.Company;
    
    public void AddProduct(Product product)
    {
        if (Products.ContainsKey(product.ProductID))
        {
            throw new ArgumentException($"Product with ID {product.ProductID} already exists in the proposal.");
        }
        if(product.DependentProduct !=null){
            if(!Products.ContainsKey(product.DependentProduct.ProductID)){
                throw new ArgumentException($"Product with ID {product.ProductID} has a dependent product that is not in the proposal.");
            }
        }
        Products.Add(product.ProductID, product);
    }

    public void FinalizeProposal()
    {
        if (Status == Status.Active)
        {
            throw new ArgumentException($"Proposal with ID {ProposalID} is already finalized.");
        }
        Status = Status.Active;
        Company.Status = Status.Active;
    }
}