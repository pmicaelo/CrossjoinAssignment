public class Lead
{
    public required Company Company { get; set; }
    public required string LeadID { get; init; }
    public string? Country => Company.Country;
    public BusinessType BusinessType { get; set; } = BusinessType.None;
    public Status Status { get; set; } = Status.Draft;
    //public Proposal? Proposal { get; set; }

    public Proposal TransformToProposal(string proposalID)
    {
        return new Proposal
        {
            ProposalID = proposalID,
            Lead = this,
        };
    }
}