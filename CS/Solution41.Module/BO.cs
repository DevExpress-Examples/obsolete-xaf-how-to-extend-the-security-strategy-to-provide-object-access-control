using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Solution41.Module {
    [DefaultClassOptions]
    public class Client : Person {
        public Client(Session s) : base(s) { }
    }

    [DefaultClassOptions]
    public class SaleTask : Task {
        public SaleTask(Session s) : base(s) { }
    }
}
