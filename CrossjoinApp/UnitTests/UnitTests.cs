using System.ComponentModel.DataAnnotations;

public class UnitTests
{
    [Fact]
    public void Proposal_Inherits_Lead_Fields()
    {
        Application app = new Application();
        
        Company company = app.CreateCompany(companyID:"company1", country: "England");
        Lead lead = app.CreateLead(leadID: "lead1", companyID: "company1");

        Assert.Equal(lead.Country, company.Country);
        Assert.Equal(lead.Company, company);
    }

    [Fact]
    public void Lead_Inherits_Company_Fields()
    {
        Application app = new Application();
        
        app.CreateCompany(companyID:"company1", country: "SPAIN", nif : "233232332");
        Lead lead = app.CreateLead(leadID: "lead1", companyID: "company1", businessType: BusinessType.Retail);
        Proposal proposal = app.CreateProposal(proposalID: "prop1", leadID: "lead1", companyID: "company1");

        Assert.Equal(lead.Country, proposal.Country);
        Assert.Equal(lead.BusinessType, proposal.BusinessType);
        Assert.Equal(lead.Company, proposal.Company);

    }

    [Fact]
    public void Portugal_NIF_Required_Success()
    {
        Application app = new Application();

        app.CreateCompany(companyID:"company1", country: "Portugal", nif: "233232332");
        
        Assert.NotNull(app.GetCompany("company1"));
    }

    [Fact]
    public void Portugal_NIF_Required_Fail()
    {
        Application app = new Application();

        Assert.Throws<ValidationException>(() => app.CreateCompany(companyID: "company1", country: "Portugal", nif: null)); 
    }

    [Fact] 
    public void Not_Portugal_NIF_Not_Required()
    {
        Application app = new Application();

         app.CreateCompany(companyID:"company1", country: "Spain");
         Assert.NotNull(app.GetCompany("company1"));
    }

    [Fact]
    public void Proposal_Missing_Dependency()
    {
        Application app = new Application();
        
        app.CreateCompany(companyID:"company1", country: "SPAIN", nif : "233232332");
        app.CreateLead(leadID: "lead1", companyID: "company1", businessType: BusinessType.Retail);
        app.CreateProposal(proposalID: "prop1", leadID: "lead1", companyID: "company1");

        app.CreateProduct(productID: "product1", dependentProductID: null, productType: ProductType.None);
        app.CreateProduct(productID: "product2", dependentProductID: "product1", productType: ProductType.None);

        Assert.Throws<ArgumentException>(() =>app.AddProductToProposal(proposalID: "prop1", productID: "product2", companyID: "company1"));

    }
    
}