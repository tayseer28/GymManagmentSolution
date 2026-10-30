# Booking System - Complete Workflow Guide

## Overview
The Booking system is fully implemented and functional. Here's how to navigate through it:

---

## How to Access the Booking System

### Entry Point: Main Navigation
In the **_Layout.cshtml** navigation bar, there's a link labeled **"Sessions Schedule"** that takes you to the Booking Index.

**Navigation Path:**
```
Navigation Menu → Sessions Schedule → Booking/Index
```

---

## Booking System Workflow

### 1️⃣ **View All Sessions (Index)**
**Action:** `Booking/Index`
**URL:** `/Booking/Index`

This page displays all sessions organized in two categories:

#### A. **Upcoming Sessions** (for creating bookings)
- Shows sessions with status "Upcoming"
- Each session displays:
  - Category Name
  - Status badge
  - Trainer Name
  - Date
  - Time Range
  - Duration
  - Available Capacity
- **Action Button:** "View Members" → Takes you to `GetMembersForUpcomingSession`

#### B. **Ongoing Sessions** (for managing attendance)
- Shows sessions with status "Ongoing"
- Similar layout to Upcoming Sessions
- **Action Button:** "View Members" → Takes you to `GetMembersForOngoingSessions`

---

### 2️⃣ **View Members for Upcoming Session**
**Action:** `GetMembersForUpcomingSession(int id)`
**URL:** `/Booking/GetMembersForUpcomingSession/{sessionId}`

This page shows all members already booked for a specific upcoming session.

**Features:**
- Table displaying:
  - Member Name
  - Booking Date
  - Actions

**Available Actions:**
- **"New Booking" Button** (Top-right header)
  - Takes you to the Create booking form
  - Passes the session ID in the URL

- **"Cancel" Button** (For each booked member)
  - Form Action: POST to `Cancel` action
  - Hidden inputs: `MemberId` and `SessionId`
  - Includes confirmation dialog: "Are you sure you want to cancel this booking?"

**Cancel Action Details:**
```csharp
[HttpPost]
public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
{
	var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
	TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = 
		result.Success ? "Booking cancelled successfully." : result.Error;
	return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
}
```

---

### 3️⃣ **Create New Booking**
**Action:** `Create(int id)` [GET] → Shows form
**Action:** `Create(CreateBookingViewModel model)` [POST] → Submits booking

**URL (GET):** `/Booking/Create/{sessionId}`
**URL (POST):** `/Booking/Create` with form data

**Form Structure:**
```
┌─ Card: Booking Information
│
├─ Member Dropdown (required)
│  └─ Populated with eligible members for that session
│     Using ViewBag.Members set in GET action
│
├─ Hidden Input: SessionId
│  └─ Automatically populated from URL route parameter
│
└─ Buttons:
   ├─ "Create Booking" (Submit)
   └─ "Back to List" (Anchor to GetMembersForUpcomingSession)
```

**Flow:**
1. Click "New Booking" button from members list
2. Fill in Member dropdown
3. Click "Create Booking"
4. If successful → Redirected back to `GetMembersForUpcomingSession` with success message
5. If error → Stay on form with error message

---

### 4️⃣ **View Members for Ongoing Session (Attendance)**
**Action:** `GetMembersForOngoingSessions(int id)`
**URL:** `/Booking/GetMembersForOngoingSessions/{sessionId}`

This page shows members booked for an ongoing session and their attendance status.

**Features:**
- Table displaying:
  - Member Name
  - Attendance Status

**Attendance Status Display:**
- ✅ **If Attended:** Shows green "Attended" badge with checkmark
- ⭕ **If Not Attended:** Shows "Mark as Attended" button

**Attended Action Details:**
```csharp
[HttpPost]
public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
{
	var result = await _bookingService.MarkAttendedAsync(memberId, sessionId, ct);
	TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = 
		result.Success ? "Attendance recorded." : result.Error;
	return RedirectToAction(nameof(GetMembersForOngoingSessions), new { id = sessionId });
}
```

---

## Complete Navigation Map

```
┌─ Home Page
│
├─ Sessions Schedule (Navigation menu)
│  │
│  └─ Booking/Index
│     ├─ Upcoming Sessions Section
│     │  └─ View Members → GetMembersForUpcomingSession/{id}
│     │     ├─ New Booking → Create/{id} [GET]
│     │     │  └─ Create Booking Form [POST] → Create/{id}
│     │     │     └─ Cancel Button Form → Cancel[POST]
│     │     └─ Back to List → Booking/Index
│     │
│     └─ Ongoing Sessions Section
│        └─ View Members → GetMembersForOngoingSessions/{id}
│           ├─ Mark as Attended → Attended[POST]
│           └─ Back to List → Booking/Index
```

---

## Controller Actions Summary

| Action | HTTP Method | Route | Purpose |
|--------|------------|-------|---------|
| `Index` | GET | `/Booking/Index` | Show all sessions (Upcoming & Ongoing) |
| `GetMembersForUpcomingSession` | GET | `/Booking/GetMembersForUpcomingSession/{id}` | Show members booked for upcoming session |
| `GetMembersForOngoingSessions` | GET | `/Booking/GetMembersForOngoingSessions/{id}` | Show members & attendance for ongoing session |
| `Create` | GET | `/Booking/Create/{id}` | Show create booking form |
| `Create` | POST | `/Booking/Create` | Submit new booking |
| `Cancel` | POST | `/Booking/Cancel` | Cancel a booking |
| `Attended` | POST | `/Booking/Attended` | Mark member as attended |

---

## Service Methods Used

The BookingService provides these methods:

```csharp
// Retrieval Methods
GetAllSessionsAsync() → All sessions for Index
GetMembersForUpcomingBySessionIdAsync(id) → For GetMembersForUpcomingSession
GetMembersForOngoingBySessionIdAsync(id) → For GetMembersForOngoingSessions
GetMembersForDropDownAsync(id) → For Create form dropdown

// Action Methods
CreateNewBookingAsync(model) → Creates booking
CancelBookingAsync(memberId, sessionId) → Cancels booking
MarkAttendedAsync(memberId, sessionId) → Records attendance
```

---

## Key Implementation Details

### Session Status
- **Upcoming**: Sessions with future dates (can add/cancel bookings)
- **Ongoing**: Sessions currently in progress (can mark attendance)

### Security & Authorization
- All actions require `[Authorize]` attribute
- Users must be logged in to access booking features

### UI Components
- Bootstrap cards for session display
- Tables with striped hover effects
- Confirmation dialogs for destructive actions (Cancel)
- Alert boxes for success/error messages (using TempData)
- Optional: Select2 enhanced dropdowns on Create form

---

## Quick Summary: How to Reach Each Feature

| Feature | How to Access |
|---------|---------------|
| **Create Booking** | Sessions Schedule → View Members (Upcoming) → New Booking button |
| **Cancel Booking** | Sessions Schedule → View Members (Upcoming) → Cancel button |
| **Mark Attended** | Sessions Schedule → View Members (Ongoing) → Mark as Attended button |
| **View All Sessions** | Sessions Schedule (main entry point) |

---

**Status:** ✅ All functionality is implemented and integrated with the UI navigation.
