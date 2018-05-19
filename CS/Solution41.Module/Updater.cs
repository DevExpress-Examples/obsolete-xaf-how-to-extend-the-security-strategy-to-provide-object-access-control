using System;

using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;

namespace Solution41.Module {
    public class Updater : ModuleUpdater {
        public Updater(DevExpress.ExpressApp.IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

            UserWithLicense admin = Session.FindObject<UserWithLicense>(CriteriaOperator.Parse("IsAdministrator==true && IsActive==true"));
            if (admin == null) {
                admin = new UserWithLicense(Session);
                admin.UserName = "Admin";
                admin.IsActive = true;
                admin.IsAdministrator = true;
                admin.Save();
            }
            UserWithLicense guest = Session.FindObject<UserWithLicense>(CriteriaOperator.Parse("UserName=='Guest'"));
            if (guest == null) {
                guest = new UserWithLicense(Session);
                guest.UserName = "Guest";
                guest.IsActive = true;
                guest.Save();
            }
            UserWithLicense user = Session.FindObject<UserWithLicense>(CriteriaOperator.Parse("UserName=='User'"));
            if (user == null) {
                user = new UserWithLicense(Session);
                user.UserName = "User";
                user.IsActive = true;
                user.License = "Solution41.Module.Client;Solution41.Module.SaleTask;";
                user.Save();
            }
        }
    }
}
