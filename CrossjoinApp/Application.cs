public class Application{
    public Dictionary<string, Company> Companies { get; } = new();
    public Dictionary<string, Product> Products { get; } = new();
    public DynamicRuleManager RuleManager { get; } = new();

    public Application()
    {
        AddRule<Company>( className: "Company", 
            fieldName: "NIF", 
            condition: company => string.Equals(company.Country, "Portugal", StringComparison.OrdinalIgnoreCase), 
            isRequired: true);
    }
    public Company CreateCompany(string companyID , string? nif = null, string? address = null, string? country = null,
    string? stakeholder = null, string? contact = null)
    {
        if (Companies.ContainsKey(companyID))
        {
            throw new ArgumentException($"Company with ID {companyID} already exists.");
        }
        Company company = new Company
        {
            ID = companyID,
            NIF = nif,
            Address = address,
            Country = country,
            Stakeholder = stakeholder,
            Contact = contact
        };
        RuleManager.Validate(company);
        
        Companies.Add(company.ID, company);
        return company;
    }

    public void UpdateCompany(string companyID, string? nif = null, string? address = null, string? country = null,
     Status? status = null, string? stakeholder = null, string? contact= null)
    {
        Company company = GetCompany(companyID);
        
        if ( nif != null) company.NIF = nif;
        if (address != null) company.Address = address;
        if (country!= null) company.Country = country;
        if (status.HasValue) company.Status = status.Value;
        if (stakeholder!= null)company.Stakeholder = stakeholder;
        if (contact!= null)company.Contact = contact;

        RuleManager.Validate(company);

    }

    public Company GetCompany(string companyID)
    {
        if (!Companies.TryGetValue(companyID, out Company? company))
        {
            throw new KeyNotFoundException($"Company with ID {companyID} does not exist.");
        }
        return company;
    }

    public Lead CreateLead(string leadID, string companyID, BusinessType businessType = BusinessType.None)
    {
        Company company = GetCompany(companyID);
        return company.GenerateLead(leadID, businessType);
    }

    public void UpdateLead(string leadID, string companyID, BusinessType? businessType = null, Status? status = null)
    {
        Company company = GetCompany(companyID);
        company.UpdateLead(leadID, businessType, status);
    }

    public Lead GetLead(string leadID, string companyID)
    {
        Company company = GetCompany(companyID);
        Lead lead = company.GetLead(leadID);
        return lead;
    }

    public Product CreateProduct(string productID, string? dependentProductID = null, ProductType productType = ProductType.None) 
    {
        if (Products.ContainsKey(productID))
        {
            throw new ArgumentException($"Product with ID {productID} already exists.");
        }
        Product? dependentProduct = null;

        if (dependentProductID != null)
        {
            if (!Products.TryGetValue(dependentProductID, out dependentProduct))
            {
                throw new KeyNotFoundException($"Product with ID {dependentProductID} does not exist.");
            }
            if (productType != dependentProduct.ProductType)
            {
                throw new ArgumentException($"Product with ID {dependentProduct.ProductID} has a different product type.");
            }
        }
        Product product = new Product
        {
            ProductID = productID,
            DependentProduct = dependentProduct,
            ProductType = productType,
        };
        Products.Add(productID, product);
        return product;
    }

    public void UpdateProduct(string productID, string? dependentProductID = null, ProductType? productType = null)
    {
        Product product = GetProduct(productID);
        Product? dependentProduct = null;
        ProductType? newProductType = productType ?? product.ProductType;
        if (dependentProductID != null)
        {
            dependentProduct = GetProduct(dependentProductID);
        }
        if (dependentProduct != null && newProductType != dependentProduct.ProductType)
        {
            throw new ArgumentException($"Product with ID {dependentProduct.ProductID} has a different product type.");
        }
        if (productType.HasValue) product.ProductType = productType.Value;
        if (dependentProduct != null) product.DependentProduct = dependentProduct;
    }
    public Product GetProduct(string productID)
    {
        if (!Products.TryGetValue(productID, out Product? product))
        {
            throw new KeyNotFoundException($"Product with ID {productID} does not exist.");
        }
        return product;
    }

    public Proposal CreateProposal(string proposalID, string leadID, string companyID)
    {
        Company company = GetCompany(companyID);
        return company.GenerateProposal(proposalID, leadID);
    }

    public void UpdateProposal(string proposalID,string companyID, Dictionary<string, Product>? products = null, double? productionCost = null, int? monthlyProducedProducts = null, double? expectedMonthlyProfit = null, Status? status = null)
    {
        Company company = GetCompany(companyID);
        company.UpdateProposal(proposalID, products, productionCost, monthlyProducedProducts, expectedMonthlyProfit, status);  
    }

    public void AddProductToProposal(string proposalID, string productID, string companyID)
    {
        Company company = GetCompany(companyID);
        Proposal proposal = company.GetProposal(proposalID);
        Product product = GetProduct(productID);
        proposal.AddProduct(product);
        RuleManager.Validate(proposal);
    }

    public void FinalizeProposal(string proposalID, string companyID)
    {
        Company company = GetCompany(companyID);
        company.FinalizeProposal(proposalID);
    }

    public void AddRule<T>(string className, string fieldName, Func<T, bool> condition, bool isRequired)
    {
        RuleManager.SetRequired(className, fieldName, condition, isRequired);
    }
}