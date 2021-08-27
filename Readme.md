<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/134075413/12.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E2184)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
# OBSOLETE - How to extend the security strategy to provide object access control, based on encrypted license data


<p>Although you can create a new security strategy from scratch, here we'll extend the SecuritySimple strategy.<br />
First, create an interface to declare the user's license.</p><br />


```cs
    public interface ILicensedUser : ISimpleUser {
        string License { get;}
    }

```

<p>Then, create a custom user class and implement this interface, or extend the existing SimpleUser class.<br />
Now, create a SecuritySimple descendant, and override the ReloadPermissions method to assign permissions, based on license data.<br />
</p>

```cs
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


```

<p>Here we implement additive permissions, based on the list of classes encoded in the license. This part can be implemented in different ways, including subtractive permissions, or any other complex scheme. You should additionally implement the DecryptLicense method according to your security requirements.<br />
To use this security strategy, open the WinApplication or WebApplication module, and replace the existing security strategy with this one. Then, set the SecuritySimpleEx.UserType property to your new user class, and add some authentication type.</p>

<br/>


