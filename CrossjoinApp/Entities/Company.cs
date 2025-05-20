public class Company 
{    
    public required string ID { get; init; }
    public string? NIF { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public Status Status { get; set; } = Status.Draft;
    public string? Stakeholder { get; set; }
    public string? Contact { get; set; }
    private readonly Dictionary<string, Lead> leads = new();
    private readonly Dictionary<string, Proposal> proposals = new();

    public Lead GenerateLead(string leadID, BusinessType businessType = BusinessType.None)
    {
        if (leads.ContainsKey(leadID))
        {
            throw new ArgumentException($"Company with ID {ID} already has the lead {leadID}.");
        }
        Lead lead = new Lead
        {
            Company = this,
            LeadID = leadID,
            BusinessType = businessType,
        };
        leads.Add(leadID, lead);
        return lead;
    }

    public void UpdateLead(string leadID, BusinessType? businessType = null, Status? status = null)
    {
        Lead lead = GetLead(leadID);
        if (businessType.HasValue) lead.BusinessType = businessType.Value;
        if (status.HasValue) lead.Status = status.Value;
    }

    public Lead GetLead(string leadID)
    {
        if (!leads.TryGetValue(leadID, out Lead? lead))
        {
            throw new KeyNotFoundException($"Company with ID {ID} does not have the lead {leadID}.");
        }
        return lead;
    }

    public List<Lead> GetAllLeads()
    {
        return leads.Values.ToList();
    }

    public Proposal GenerateProposal(string proposalID, string leadID)
    {
        Lead lead = GetLead(leadID);
        if (proposals.ContainsKey(proposalID))
        {
            throw new ArgumentException($"Proposal with ID {proposalID} already exists.");
        }
        Proposal proposal = lead.TransformToProposal(proposalID);
        proposals.Add(proposalID, proposal);
        return proposal;
    }

    public void UpdateProposal(string proposalID, Dictionary<string, Product>? products = null, double? productionCost = null, int? monthlyProducedProducts = null, double? expectedMonthlyProfit = null, Status? status = null)
    {
        Proposal proposal = GetProposal(proposalID);
        if (products != null) proposal.Products = products;
        if (productionCost!=null) proposal.ProductionCost = productionCost;
        if (monthlyProducedProducts!=null) proposal.MonthlyProducedProducts = monthlyProducedProducts;
        if (expectedMonthlyProfit!=null) proposal.ExpectedMonthlyProfit = expectedMonthlyProfit;
        if (status.HasValue) proposal.Status = status.Value;
    }

    public void FinalizeProposal(string proposalID)
    {
        Proposal proposal = GetProposal(proposalID);
        if (proposal.Status == Status.Active)
        {   
            throw new ArgumentException($"Proposal with ID {proposal.ProposalID} is already finalized.");
        }
        proposal.FinalizeProposal();
    }

    public Proposal GetProposal(string proposalID)
    {
        if (!proposals.TryGetValue(proposalID, out Proposal? proposal))
        {
            throw new KeyNotFoundException($"Company with ID {ID} does not have the proposal {proposalID}.");
        }
        return proposal;
    }

    public List<Proposal> GetAllProposals()
    {
        return proposals.Values.ToList();
    }

}