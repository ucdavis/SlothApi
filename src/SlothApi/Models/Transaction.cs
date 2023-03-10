
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SlothApi.Models;

public class Transaction
{
    public Transaction()
    {
        Transfers = new List<Transfer>();
        Metadata = new List<TransactionMetadata>();
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Description { get; set; } = "";


    // Status updates must go through SetStatus to ensure StatusEvents are properly updated
    [MaxLength(20)]
    public string Status { get; private set; } = "";

    /// <summary>
    /// Tracking Number created by the merchant accountant
    /// </summary>
    [MaxLength(128)]
    [DisplayName("Merchant Tracking Number")]
    public string MerchantTrackingNumber { get; set; } = "";

    /// <summary>
    /// URL created by the merchant to track back
    /// </summary>
    [Display(Name = "Merchant Url")]
    public string MerchantTrackingUrl { get; set; } = "";

    /// <summary>
    /// Tracking Number created by the payment processor
    /// </summary>
    [MaxLength(128)]
    [DisplayName("Processor Tracking Number")]
    public string ProcessorTrackingNumber { get; set; } = "";

    /// <summary>
    /// Unique identifier for a set of related transactions per origination code.
    /// A file can have multiple document numbers but the file must balance by document number(aka net zero) and by total amount.Debits = Credits
    /// Once a document number posts to the general ledger then it cannot be used again.
    /// </summary>
    [MinLength(1)]
    [MaxLength(14)]
    [RegularExpression("[A-Z0-9]*")]
    [DisplayName("Document Number")]
    public string DocumentNumber { get; set; } = "";

    /// <summary>
    /// Primarily used in Decision Support reporting for additional transaction identification.
    /// Equivalent to the KFS Organization Document Number.
    /// </summary>
    [MinLength(1)]
    [MaxLength(10)]
    [DisplayName("Kfs Tracking Number")]
    public string KfsTrackingNumber { get; set; } = "";

    /// <summary>
    /// Date the transaction occurred.
    /// </summary>
    [Required]
    [DisplayName("Transaction Date")]
    public DateTime TransactionDate { get; set; }

    public IList<Transfer> Transfers { get; set; }


    public string ScrubberId { get; set; } = "";

    [DisplayName("Reversal for Transaction")]
    public Transaction? ReversalOfTransaction { get; set; }

    [DisplayName("Reversal for Transaction Id")]
    public string? ReversalOfTransactionId { get; set; }

    [NotMapped]
    [DisplayName("Is Reversal Transaction")]
    public bool IsReversal => !string.IsNullOrEmpty(ReversalOfTransactionId);

    [DisplayName("Reversal Transaction")]
    public Transaction? ReversalTransaction { get; set; }

    [DisplayName("Reversal Transaction Id")]
    public string? ReversalTransactionId { get; set; }

    [NotMapped]
    [DisplayName("Has Reversal Transaction")]
    public bool HasReversal => !string.IsNullOrEmpty(ReversalTransactionId);

    public IList<TransactionMetadata> Metadata { get; set; }

    public void AddReversalTransaction(Transaction transaction)
    {
        // setup bidirectional relationship
        this.ReversalTransaction = transaction;
        transaction.ReversalOfTransaction = this;
    }
}
