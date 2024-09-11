using AssetManagement.Data;
using AssetManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManagement.Services
{
    public class ProfileAccessService
    {
        private readonly AssetManagementContext dbContext;

        public ProfileAccessService(AssetManagementContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CreateProfileAccesses()
        {
            // Retrieve profiles and modules from the database
            List<Profile> profiles = dbContext.tbl_ictams_profiles.ToList();
            List<Module> modules = dbContext.tbl_ictams_modules.ToList();

            // Retrieve existing profile access records from the database
            List<ProfileAccess> existingProfileAccesses = dbContext.tbl_ictams_profileaccess.ToList();

            // Generate access records for each combination of profiles and modules
            List<ProfileAccess> profileAccesses = new List<ProfileAccess>();
            foreach (Profile profile in profiles)
            {
                foreach (Module module in modules)
                {
                    // Check if a profile access record already exists for the combination of profile and module
                    bool accessExists = existingProfileAccesses.Any(pa => pa.ProfileId == profile.ProfileId && pa.ModuleId == module.ModuleId);
                    if (accessExists)
                    {
                        // Skip creating the profile access if it already exists
                        continue;
                    }

                    ProfileAccess access = new ProfileAccess
                    {
                        ProfileId = profile.ProfileId,
                        ModuleId = module.ModuleId,
                        OpenAccess = "N", // Set the default access as "N" (no access)
                        UserCreated = "ICT01",
                        DateCreated = DateTime.Now
                    };
                    profileAccesses.Add(access);
                }
            }

            // Save the new access records to the database
            dbContext.tbl_ictams_profileaccess.AddRange(profileAccesses);
            dbContext.SaveChanges();
        }

        public void CreateProfileAccessForNewProfile(Profile newProfile)
        {
            List<Module> modules = dbContext.tbl_ictams_modules.ToList();
            List<ProfileAccess> profileAccesses = new List<ProfileAccess>();

            foreach (Module module in modules)
            {
                ProfileAccess access = new ProfileAccess
                {
                    ProfileId = newProfile.ProfileId,
                    ModuleId = module.ModuleId,
                    OpenAccess = "N", // Set the default access as "N" (no access)
                    UserCreated = "ICT01",
                    DateCreated = DateTime.Now
                };
                profileAccesses.Add(access);
            }

            dbContext.tbl_ictams_profileaccess.AddRange(profileAccesses);
            dbContext.SaveChanges();
        }

        public void CreateProfileAccessForNewModule(Module newModule)
        {
            List<Profile> profiles = dbContext.tbl_ictams_profiles.ToList();
            List<ProfileAccess> profileAccesses = new List<ProfileAccess>();

            foreach (Profile profile in profiles)
            {
                ProfileAccess access = new ProfileAccess
                {
                    ProfileId = profile.ProfileId,
                    ModuleId = newModule.ModuleId,
                    OpenAccess = "N", // Set the default access as "N" (no access)
                    UserCreated = "ICT01",
                    DateCreated = DateTime.Now
                };
                profileAccesses.Add(access);
            }

            dbContext.tbl_ictams_profileaccess.AddRange(profileAccesses);
            dbContext.SaveChanges();
        }
    }

}
