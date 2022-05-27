using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Signature
{
    [Serializable]
    public class Gepg
    {
        public gepgBillSubReq gepgBillSubReq { get; set; }
        public string gepgSignature {get;set;}
    }

    [Serializable]
    public class gepgBillSubReq
    {
        public BillHdr BillHdr { get; set; }
        public BillTrxInf BillTrxInf { get; set; }
    }

    public class BillHdr
    {
        public String SpCode { get; set; }
        public String RtrRespFlg { get; set; }

    }
    
    public class BillTrxInf
    {
        public String BillId { get; set; }
        public String SubSpCode { get; set; }
        public String SpSysId { get; set; }
        public Double BillAmt { get; set; }
        public Double MiscAmt { get; set; }
        public String BillExprDt { get; set; }
        public String PyrId { get; set; }
        public String PyrName { get; set; }
        public String BillDesc { get; set; }
        public String BillGenDt { get; set; }
        public int BillGenBy { get; set; }
        public String BillApprBy { get; set; }
        public String PyrCellNum { get; set; }
        public String PyrEmail { get; set; }
        public String Ccy { get; set; }
        public Double BillEqvAmt { get; set; }
        public String RemFlag { get; set; }
        public int BillPayOpt { get; set; }
        public List<BillItem> BillItems { get; set; }
    }

    public class BillItem
    {
        public String BillItemRef { get; set; }
        public String UseItemRefOnPay { get; set; }
        public Double BillItemAmt { get; set; }
        public Double BillItemEqvAmt { get; set; }
        public Double BillItemMiscAmt { get; set; }
        public String GfsCode { get; set; }
    }
}