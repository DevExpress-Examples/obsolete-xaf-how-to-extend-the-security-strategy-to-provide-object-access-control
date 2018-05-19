using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Base.Security;
using System.Security;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
using System.ComponentModel;

namespace Solution41.Module {

    public interface ILicensedUser : ISimpleUser {
        string License { get;}
    }

    [DefaultClassOptions]
    public class UserWithLicense : SimpleUser, ILicensedUser {

        private string license;

        public UserWithLicense(Session s) : base(s) { }

        #region ILicensedUser Members

        [Browsable(false)]
        public string License {
            get { return license; }
            set { SetPropertyValue("License", ref license, value); }
        }

        #endregion
    }

    public class SecuritySimpleEx : SecuritySimple {
        public SecuritySimpleEx() : base() { }
        public SecuritySimpleEx(Type userType, AuthenticationBase authentication)
            : base(userType, authentication) {
        }
        public override bool IsSecurityMember(Type type, string memberName) {
            return ((typeof(ILicensedUser).IsAssignableFrom(type) && (typeof(ILicensedUser).GetMember(memberName).Length > 0)) || base.IsSecurityMember(type, memberName));
        }
        protected override PermissionSet ReloadPermissions() {
            PermissionSet set = base.ReloadPermissions();
            if (this.User.IsActive) {
                base.IsGrantedForNonExistentPermission = true;
                set.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                set.AddPermission(new ObjectAccessPermission(typeof(IPropertyBag), ObjectAccess.AllAccess));
                set.AddPermission(new ObjectAccessPermission(typeof(IXPSimpleObject), ObjectAccess.NoAccess));
                set.AddPermission(new ObjectAccessPermission(this.UserType, ObjectAccess.AllAccess));
                set.AddPermission(new EditModelPermission(this.User.IsAdministrator ? ModelAccessModifier.Allow : ModelAccessModifier.Deny));
                if (!this.User.IsAdministrator) {
                    set.AddPermission(new ObjectAccessPermission(this.UserType, ObjectAccess.ChangeAccess, ObjectAccessModifier.Deny));
                    if (!this.AllowNonAdministratorNavigateToUsers) {
                        set.AddPermission(new ObjectAccessPermission(this.UserType, ObjectAccess.Navigate, ObjectAccessModifier.Deny));
                    }
                }
                ILicensedUser luser = this.User as ILicensedUser;
                if (luser != null && !string.IsNullOrEmpty(luser.License)) {
                    AddLicensedPermissions(set, DecryptLicense(luser.License));
                }
                return set;
            }
            base.IsGrantedForNonExistentPermission = false;
            return set;
        }

        private string DecryptLicense(string license) {
            return license;
        }

        private void AddLicensedPermissions(PermissionSet set, string license) {
            if (license == null) return;
            string[] items = license.Split(';');
            foreach (string item in items) {
                Type type = Type.GetType(item);
                if (type != null) {
                    ObjectAccessPermission p = new ObjectAccessPermission(type, ObjectAccess.AllAccess);
                    set.AddPermission(p);
                }
            }
        }
    }
}
