import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

function Patients() {
    const navigate = useNavigate();

    //const [Date, setDate] = useState('');
    //const [Notes, setNotes] = useState('');
    //const [PatientId, setPatientId] = useState('');
    //const [Patients, setPatients] = useState([]);

    //useEffect(() => {
    //    populatePatients();
    //}, [Patients]);

    //const populatePatients = async () => {
    //    const token = await authService.getAccessToken();
    //    const user = await authService.getUser();
    //    const response = await fetch('patient', {
    //        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    //    })
    //    const data = await response.json();
    //    setPatients(data);
    //}

    //let handlePatientChange = (e) => {
    //    setPatientId(e.target.value)
    //}

    //const handleSubmit = async (event) => {
    //    event.preventDefault();
    //    //const token = await authService.getAccessToken();
    //    const response = await fetch('patient',
    //        {
    //            method: 'POST',
    //            body: JSON.stringify({ Date, Notes, PatientId }),
    //            headers: { 'Content-Type': 'application/json' }
    //        }
    //    );
    //    props.history.push("/visits");
    //}

    //return (
    //    <div className="container">
    //        <h1>Create Visit Entry</h1>
    //        <div className="row">
    //            <div className="col-mid-6">
    //                <form id="statsForm" onSubmit={handleSubmit}>
    //                    <div className="form-group">
    //                        <label htmlFor="visitDate">Visit Date</label>
    //                        <input type="date" name="visitDate" className="form-control" onChange={(e) => setDate(e.target.value)} />
    //                    </div>
    //                    <div className="form-group">
    //                        <label htmlFor="notes">Notes</label>
    //                        <input type="text" name="notes" className="form-control" onChange={(e) => setNotes(e.target.value)} />
    //                    </div>
    //                    <select onChange={handlePatientChange}>
    //                        <option value="⬇️ Select a patient ⬇️"> -- Select a patient -- </option>
    //                        {Patients.map((patient) => <option value={patient.patientId}>{patient.patientName} | {patient.patientId}</option>)}
    //                    </select>
    //                    <div className="form-group">
    //                        <button type="submit" className="btn btn-success">Submit</button>
    //                    </div>
    //                </form>
    //            </div>
    //        </div>
    //    </div>
    //);

    const [Patients, setPatients] = useState([]);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [loading, setLoading] = useState(true);
    const [doctorId, setDoctorId] = useState(null);
    const [hiddenPatientIds, setHiddenPatientIds] = useState(null);
    const [addedPatientIds, setAddedPatientIds] = useState(null);
    const [storageLoaded, setStorageLoaded] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);
    const [allPatients, setAllPatients] = useState([]);
    const [selectedPatientId, setSelectedPatientId] = useState('');

    // Load hidden and added patient IDs from localStorage on mount
    useEffect(() => {
        const storedHidden = localStorage.getItem('hiddenPatientIds');
        const storedAdded = localStorage.getItem('addedPatientIds');

        if (storedHidden) {
            try {
                setHiddenPatientIds(JSON.parse(storedHidden));
            } catch (e) {
                console.error('Failed to parse hidden patient IDs from localStorage', e);
                setHiddenPatientIds([]);
            }
        } else {
            setHiddenPatientIds([]);
        }

        if (storedAdded) {
            try {
                setAddedPatientIds(JSON.parse(storedAdded));
            } catch (e) {
                console.error('Failed to parse added patient IDs from localStorage', e);
                setAddedPatientIds([]);
            }
        } else {
            setAddedPatientIds([]);
        }

        setStorageLoaded(true);
    }, []);

    useEffect(() => {
        // Don't initialize until localStorage is loaded
        if (!storageLoaded) return;

        const initialize = async () => {
            const authenticated = await authService.isAuthenticated();
            setIsAuthenticated(authenticated);

            if (authenticated) {
                const currentUser = await authService.getUser();
                const role = currentUser.role || 'User';
                setUserRole(role);

                await populatePatients(currentUser, role);
            }
            setLoading(false);
        };

        initialize();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [storageLoaded]);

    const handleViewVisits = (patientId) => {
        // Navigate to visits page filtered by doctor and patient
        navigate(`/visiturl?doctorId=${doctorId}&patientId=${patientId}`);
    }

    const handleRemovePatient = (patientId, isAddedPatient) => {
        const message = isAddedPatient
            ? 'Remove this patient from your list?'
            : 'Remove this patient from your list? (Visit history will be preserved)';

        if (window.confirm(message)) {
            if (isAddedPatient) {
                // Remove from added list
                const newAddedIds = addedPatientIds.filter(id => id !== patientId);
                setAddedPatientIds(newAddedIds);
                localStorage.setItem('addedPatientIds', JSON.stringify(newAddedIds));
            } else {
                // Add to hidden list
                const newHiddenIds = [...hiddenPatientIds, patientId];
                setHiddenPatientIds(newHiddenIds);
                localStorage.setItem('hiddenPatientIds', JSON.stringify(newHiddenIds));
            }

            // Update displayed patients
            setPatients(Patients.filter(pat => pat.patientId !== patientId));
        }
    }

    const handleShowAddModal = async () => {
        // Fetch all patients for the dropdown
        try {
            const token = await authService.getAccessToken();
            const response = await fetch('patient', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                credentials: 'include'
            });

            if (response.ok) {
                const patients = await response.json();
                setAllPatients(patients);
                setShowAddModal(true);
            }
        } catch (error) {
            console.error('Failed to fetch all patients:', error);
        }
    }

    const handleAddPatient = async () => {
        if (!selectedPatientId) return;

        const patientId = parseInt(selectedPatientId);

        // Check if already in the list
        const currentPatientIds = Patients.map(p => p.patientId);
        if (currentPatientIds.includes(patientId)) {
            alert('This patient is already in your list');
            return;
        }

        // Add to localStorage
        const newAddedIds = [...addedPatientIds, patientId];
        setAddedPatientIds(newAddedIds);
        localStorage.setItem('addedPatientIds', JSON.stringify(newAddedIds));

        // Remove from hidden list if it was there
        if (hiddenPatientIds.includes(patientId)) {
            const newHiddenIds = hiddenPatientIds.filter(id => id !== patientId);
            setHiddenPatientIds(newHiddenIds);
            localStorage.setItem('hiddenPatientIds', JSON.stringify(newHiddenIds));
        }

        // Find the patient and add to display
        const patientToAdd = allPatients.find(p => p.patientId === patientId);
        if (patientToAdd) {
            setPatients([...Patients, { ...patientToAdd, isAdded: true }]);
        }

        // Close modal and reset
        setShowAddModal(false);
        setSelectedPatientId('');
    }

    const populatePatients = async (currentUser, role) => {
        console.log("Populate Patients function loaded");
        console.log("Current user:", currentUser);
        console.log("User role:", role);
        try {
            const token = await authService.getAccessToken();
            console.log("Access token:", token ? "Token exists (length: " + token.length + ")" : "No token");
            let response;

            if (role === 'Doctor') {
                // Fetch doctor entity to get doctorId
                console.log("Fetching all doctors...");
                const headers = !token ? {} : { 'Authorization': `Bearer ${token}` };
                console.log("Request headers:", headers);
                const doctorResponse = await fetch('doctor', {
                    headers: headers,
                    credentials: 'include'
                });

                if (!doctorResponse.ok) {
                    console.error("Failed to fetch doctors:", doctorResponse.status, doctorResponse.statusText);
                    setPatients([]);
                    return;
                }

                const doctorsText = await doctorResponse.text();
                console.log("Raw doctors response:", doctorsText);

                let doctors;
                try {
                    doctors = JSON.parse(doctorsText);
                    console.log("All doctors:", doctors);
                    console.log("Looking for userId:", currentUser.sub);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    console.error("Response text was:", doctorsText);
                    setPatients([]);
                    return;
                }

                // Find the doctor entity linked to this user
                const currentDoctor = doctors.find(d => d.userId === currentUser.sub);
                console.log("Found doctor:", currentDoctor);

                if (currentDoctor) {
                    // Store doctorId for later use
                    setDoctorId(currentDoctor.doctorId);

                    // Fetch patients for this doctor
                    console.log(`Fetching patients for doctor ${currentDoctor.doctorId}...`);
                    response = await fetch(`doctor/${currentDoctor.doctorId}/patient`, {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (!response.ok) {
                        console.error("Failed to fetch patients:", response.status, response.statusText);
                        const errorText = await response.text();
                        console.error("Error response:", errorText);
                        setPatients([]);
                        return;
                    }
                } else {
                    console.log("No doctor entity found for user");
                    setPatients([]);
                    return;
                }
            } else if (role === 'Patient') {
                // Patients shouldn't see this page, but if they do, show empty
                setPatients([]);
                return;
            } else {
                // For other roles, show all patients
                response = await fetch('patient', {
                    method: 'GET',
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });
            }

            if (response && response.ok) {
                const responseText = await response.text();
                console.log("Raw patients response:", responseText);

                try {
                    const data = JSON.parse(responseText);
                    console.log("Parsed patients data:", data);

                    // Filter out hidden patients based on localStorage
                    const visiblePatients = data.filter(pat => !hiddenPatientIds.includes(pat.patientId));

                    // Merge with added patients from localStorage
                    await mergeAddedPatients(visiblePatients);
                } catch (e) {
                    console.error("Failed to parse patients JSON:", e);
                    console.error("Response text was:", responseText);
                    setPatients([]);
                }
            }
        } catch (error) {
            console.error("Error fetching patients:", error);
        }
    }

    const mergeAddedPatients = async (backendPatients) => {
        try {
            // If no added patients, just set backend patients
            if (!addedPatientIds || addedPatientIds.length === 0) {
                setPatients(backendPatients);
                return;
            }

            // Fetch details for added patients
            const token = await authService.getAccessToken();
            const addedPatientDetails = [];

            for (const patientId of addedPatientIds) {
                // Skip if already in backend patients or hidden
                if (backendPatients.find(p => p.patientId === patientId) || hiddenPatientIds.includes(patientId)) {
                    continue;
                }

                const response = await fetch(`patient/${patientId}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });

                if (response.ok) {
                    const patient = await response.json();
                    addedPatientDetails.push({ ...patient, isAdded: true });
                }
            }

            // Combine backend and added patients
            setPatients([...backendPatients, ...addedPatientDetails]);
        } catch (error) {
            console.error('Error merging added patients:', error);
            setPatients(backendPatients);
        }
    }

    const tableRows = Patients.map((val, key) => {
        return (
            <tr key={key}>
                <td>
                    {val.patientName}
                    {val.isAdded && <span className="badge bg-info ms-2">Added</span>}
                </td>
                <td>{val.patientEmail}</td>
                <td>
                    <button
                        className="btn btn-primary btn-sm"
                        onClick={() => handleViewVisits(val.patientId)}
                    >
                        View Visits
                    </button>
                </td>
                <td>
                    <button
                        className="btn btn-danger btn-sm"
                        onClick={() => handleRemovePatient(val.patientId, val.isAdded)}
                    >
                        Remove
                    </button>
                </td>
            </tr>
        );
    });

    // Filter available patients for the dropdown (exclude already displayed ones)
    const availablePatients = allPatients.filter(pat => {
        const isDisplayed = Patients.find(p => p.patientId === pat.patientId);
        return !isDisplayed;
    });

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <div className="Patients">
                <h1>Patients</h1>
                <p>Please log in to view patients.</p>
            </div>
        );
    }

    return (
        <div className="Patients">
            <h1>Patients</h1>

            {userRole === 'Doctor' && Patients.length === 0 && (
                <p>No patients assigned yet. Patients will be automatically assigned when you first log in.</p>
            )}

            {userRole === 'Patient' && (
                <p>This page is for doctors only.</p>
            )}

            {Patients.length > 0 && (
                <>
                    <table className="table table-striped table-hover">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Visits</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {tableRows}
                        </tbody>
                    </table>
                    <button className="btn btn-success mt-3" onClick={handleShowAddModal}>
                        Add Patient
                    </button>
                </>
            )}

            {Patients.length === 0 && userRole === 'Doctor' && (
                <button className="btn btn-success mt-3" onClick={handleShowAddModal}>
                    Add Patient
                </button>
            )}

            {/* Add Patient Modal */}
            {showAddModal && (
                <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Add Patient</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={() => setShowAddModal(false)}
                                ></button>
                            </div>
                            <div className="modal-body">
                                <label htmlFor="patientSelect" className="form-label">Select a patient:</label>
                                <select
                                    id="patientSelect"
                                    className="form-select"
                                    value={selectedPatientId}
                                    onChange={(e) => setSelectedPatientId(e.target.value)}
                                >
                                    <option value="">-- Choose a patient --</option>
                                    {availablePatients.map(pat => (
                                        <option key={pat.patientId} value={pat.patientId}>
                                            {pat.patientName} ({pat.patientEmail})
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="modal-footer">
                                <button
                                    type="button"
                                    className="btn btn-secondary"
                                    onClick={() => setShowAddModal(false)}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={handleAddPatient}
                                    disabled={!selectedPatientId}
                                >
                                    Add
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Patients;