// See https://aka.ms/new-console-template for more information

Application app = new Application();
Company? selectedCompany = null;

while (true)
{
    Console.WriteLine("\nChoose an action:\n");
    Console.WriteLine("1. Create Company");
    Console.WriteLine("2. Select Company");
    Console.WriteLine("3. Create Product");
    if (selectedCompany != null)
    {
        Console.WriteLine($"4. Create Lead (for company '{selectedCompany.ID}')");
        Console.WriteLine($"5. Create Proposal (for company '{selectedCompany.ID}')");
        Console.WriteLine($"6. Add Product to Proposal (for company '{selectedCompany.ID}')");
        Console.WriteLine($"7. Finalize Proposal (for company '{selectedCompany.ID}')");
        Console.WriteLine($"8. List details (for company '{selectedCompany.ID}')");
    }
    Console.WriteLine("0. Exit\n");

    if (selectedCompany != null)
    {
        Console.WriteLine($"Selected Company: {selectedCompany.ID}");
    }

    Console.Write("Enter option: ");
    var input = Console.ReadLine();

    try
    {
        switch (input)
        {
            case "1":
                Console.Write("Enter Company ID: ");
                var companyID = Console.ReadLine()?? "";

                Console.Write("Enter Country: ");
                var country = Console.ReadLine();

                Console.Write("Enter NIF: ");
                var nif = Console.ReadLine();

                var company = app.CreateCompany(
                    companyID,
                    nif: string.IsNullOrWhiteSpace(nif) ? null : nif,
                    country: country
                );

                selectedCompany = company;
                Console.WriteLine($"\nCompany '{company.ID}' created and selected.");
                break;

            case "2":
                Console.Write("Enter Company ID to select: ");
                companyID = Console.ReadLine() ?? "";
                selectedCompany = app.GetCompany(companyID);
                Console.WriteLine($"\nSelected company changed to '{selectedCompany.ID}'");
                break;

            case "3":
                Console.Write("Enter Product ID: ");
                var productID = Console.ReadLine() ?? "" ;

                Console.Write("Enter Dependent Product ID: ");
                var dependentProductID = Console.ReadLine();

                Console.Write($"Enter Product Type ({string.Join(", ", Enum.GetNames<ProductType>())}): ");
                var ptInput = Console.ReadLine();

                bool isValidType = Enum.TryParse<ProductType>(ptInput, ignoreCase: true, out var resultType);
                ProductType productType = isValidType ? resultType : ProductType.None;

                app.CreateProduct(productID, string.IsNullOrWhiteSpace(dependentProductID) ? null : dependentProductID, productType);

                Console.WriteLine($"\nProduct '{productID}' created.");
                break;

            case "4":
                if (selectedCompany == null)
                {
                    goto default;
                }

                Console.Write("Enter Lead ID: ");
                var leadID = Console.ReadLine()?? "";

                Console.Write($"Enter Business Type ({string.Join(", ", Enum.GetNames<BusinessType>())}): ");
                var btInput = Console.ReadLine();
                
                bool isValidBusinessType = Enum.TryParse<BusinessType>(btInput, ignoreCase: true, out var parsedBusinessType);
                BusinessType businessType = isValidBusinessType ? parsedBusinessType : BusinessType.None;

                app.CreateLead(leadID, selectedCompany.ID, businessType);
                Console.WriteLine($"\nLead '{leadID}' created for company '{selectedCompany.ID}'");
                break;

            case "5":
                if (selectedCompany == null)
                {
                    goto default;
                }

                Console.Write("Enter Proposal ID: ");
                var proposalID = Console.ReadLine()??"";

                Console.Write("Enter Lead ID: ");
                leadID = Console.ReadLine()?? "";

                var proposal = app.CreateProposal(proposalID, leadID, selectedCompany.ID);
                Console.WriteLine($"\nProposal '{proposal.ProposalID}' created for company '{selectedCompany.ID}'");
                break;

            case "6":
                if (selectedCompany == null)
                {
                    goto default;
                }

                Console.Write("Enter Proposal ID: ");
                proposalID = Console.ReadLine() ?? "";

                Console.Write("Enter Product ID to Add: ");
                productID = Console.ReadLine() ?? "";

                app.AddProductToProposal(proposalID, productID, selectedCompany.ID);
                Console.WriteLine($"\nProduct '{productID}' added to proposal '{proposalID}'.");
                break;

            case "7":
                if (selectedCompany == null)
                {
                    goto default;
                }

                Console.Write("Enter Proposal ID to finalize: ");
                proposalID = Console.ReadLine() ?? "";

                app.FinalizeProposal(proposalID, selectedCompany.ID);
                Console.WriteLine($"\nProposal '{proposalID}' finalized for company '{selectedCompany.ID}'.");
                break;
            
            case "8":
                if (selectedCompany == null)
                {
                    goto default;
                }

                Console.WriteLine($"\nCompany ID: {selectedCompany.ID}");
                Console.WriteLine($"Country: {selectedCompany.Country}");
                Console.WriteLine($"NIF: {selectedCompany.NIF ?? "None"}");
                Console.WriteLine($"Status: {selectedCompany.Status}");

                Console.WriteLine("\nLeads:");
                foreach (var lead in selectedCompany.GetAllLeads())
                {
                    Console.WriteLine($"- {lead.LeadID} | Status: {lead.Status} | Business Type: {lead.BusinessType}");
                }

                Console.WriteLine("\nProposals:");
                foreach (var propos in selectedCompany.GetAllProposals())
                {
                    var productList = propos.Products.Keys.Any() ? string.Join(", ", propos.Products.Keys) : "No Products";
                    Console.WriteLine($"- {propos.ProposalID} | Status: {propos.Status} | Products: {productList}");
                }

                break;

            case "0":
                Console.WriteLine("\nExiting.");
                return;

            default:
                Console.WriteLine("\nInvalid option.");
                break;
        }
    }
    catch (Exception exception)
    {
        Console.WriteLine($"\nError: {exception.Message}");
    }
}