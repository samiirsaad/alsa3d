namespace AlSa3d.Core.Entities;

public enum InvoiceType { Sales = 0, Purchase = 1 }
public enum InvoiceStatus { Draft = 0, Pending = 1, Paid = 2, Cancelled = 3, Approved = 4 }
public enum ReturnStatus { Pending = 0, Approved = 1, Rejected = 2 }
public enum SalaryStatus { Pending = 0, Paid = 1, Cancelled = 2 }
public enum AttendanceType { CheckIn = 0, CheckOut = 1, Absent = 2, Late = 3 }
public enum TransactionType { Deposit = 0, Withdrawal = 1, Transfer = 2, Credit = 3, Debit = 4 }
public enum CheckStatus { Pending = 0, Cashed = 1, Deposited = 2, Bounced = 3, Cancelled = 4 }
public enum DiscountType { Percentage = 0, Fixed = 1 }
