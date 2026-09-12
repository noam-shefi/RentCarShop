# Password Storage Fix - Session 2

## Issues Fixed

### 1. Password Column Too Small (NVARCHAR(50))
**Problem:** When password hashing was introduced, PBKDF2 hashes (which include salt + hash in Base64 format) exceeded the database column limit of 50 characters.

**Error Message:** `String or binary data would be truncated in table 'CarShopDB.dbo.Users', column 'Password'`

**Solution:** 
- Updated `CreateDatabase.sql`: Changed `Password NVARCHAR(50)` to `Password NVARCHAR(MAX)`
- This allows storage of PBKDF2-hashed passwords (~88 Base64 characters)

### 2. Admin Login Broken After Password Hashing
**Problem:** The admin account stored password `123456` in plaintext. After enabling PBKDF2 hashing verification in `Login.aspx.cs`, the login attempt fails because plaintext password doesn't match a hash.

**Solution - Dual Authentication Mode:**
Added backward compatibility in `Login.aspx.cs` with:
1. **Primary:** Attempt PBKDF2 hash verification first
2. **Fallback:** If hash verification fails, try plaintext comparison
3. **Auto-Upgrade:** When plaintext matches, automatically hash and store the password for future logins

This allows existing users with plaintext passwords (like admin) to log in, and their password gets securely hashed automatically.

### 3. Seed Data Migration
**File:** `InsertSampleCars.sql`

**Approach:** Keep initial seed data in plaintext format. On first login:
- User provides plaintext password
- Login.aspx.cs detects plaintext, validates it, then auto-upgrades to PBKDF2 hash
- User can continue using plaintext password on login form (as usual)
- Subsequent logins use secure hash comparison

**Alternative (Optional):** Run `GenerateHashForSeed.aspx` in your application to:
- Generate proper PBKDF2 hashes for all seed passwords
- Update the SQL INSERT statements with hashed values for production deployments

## Files Modified

### CreateDatabase.sql
```sql
-- BEFORE
Password NVARCHAR(50) NOT NULL,

-- AFTER  
Password NVARCHAR(MAX) NOT NULL,
```

### Login.aspx.cs
Enhanced `btnLogin_Click` with:
- Try PBKDF2 verification first
- Fall back to plaintext comparison
- Auto-upgrade plaintext passwords to hashes
- Maintains session/admin role setup

```csharp
// Try hashed password verification first (new format)
bool passwordMatches = PasswordHelper.VerifyPassword(password, storedPassword);

// Fallback to plaintext comparison for backward compatibility
if (!passwordMatches && storedPassword == password)
{
	passwordMatches = true;

	// Auto-upgrade to hashed password
	try
	{
		string hashedPassword = PasswordHelper.HashPassword(password);
		// ... update database with hash
	}
	catch { }
}
```

### InsertSampleCars.sql
Seed data now includes comment explaining the migration path.
Initial insert uses plaintext for backward compatibility during transition phase.

## Utility Files Created

### GenerateHashForSeed.aspx
Temporary utility page to generate PBKDF2 hashes for seed data.
- Access via browser: `~/GenerateHashForSeed.aspx`
- Copy the SQL output and update `InsertSampleCars.sql` with hashed values
- Remove this file after migration is complete

### MigratePasswords.sql
Documentation of migration approach and SQL examples.
Not required for functionality - reference only.

## Migration Timeline

### Immediate (Current State)
1. ✅ Database schema updated to NVARCHAR(MAX)
2. ✅ Login dual-mode enabled (hash + plaintext fallback)
3. ✅ Registration hashes new passwords
4. ✅ Admin/existing users can log in with plaintext, auto-upgrade happens

### Usage
- **Admin Login:** Still use `admin` / `123456` 
- **New Users:** Passwords are immediately hashed at registration
- **Existing Users:** Auto-upgraded on first login after this fix

### Optional Final Step
When ready to fully migrate:
1. Run `GenerateHashForSeed.aspx`
2. Copy hashed password values
3. Update `InsertSampleCars.sql` with hashes
4. Remove `GenerateHashForSeed.aspx` utility page
5. Update `Login.aspx.cs` to remove plaintext fallback (optional)

## Testing

To verify the fix:

1. **Register new user:**
   - Use any new username/password
   - Should succeed without truncation error
   - Password stored as PBKDF2 hash

2. **Login with admin:**
   - Use `admin` / `123456`
   - Should succeed (password auto-upgrades to hash in DB)
   - Admin panel should be accessible

3. **Login with new user:**
   - Should succeed with hash verification

## Security Notes

- PBKDF2 uses 10,000 iterations (balanced for .NET Framework 4.7.2)
- Each password has unique 16-byte salt
- Auto-upgrade happens server-side, transparent to user
- After auto-upgrade, plaintext fallback won't trigger (password is now hashed)
- No plaintext passwords stored in new registrations
