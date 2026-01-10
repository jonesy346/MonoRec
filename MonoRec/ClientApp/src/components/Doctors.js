import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

function Doctors() {

    const navigate = useNavigate();
    const [Doctors, setDoctors] = useState([]);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [loading, setLoading] = useState(true);
    const [patientId, setPatientId] = useState(null);
    const [hiddenDoctorIds, setHiddenDoctorIds] = useState(null); // null means not yet loaded
    const [addedDoctorIds, setAddedDoctorIds] = useState(null); // UI-only added doctors
    const [storageLoaded, setStorageLoaded] = useState(false);
    const [showAddModal, setShowAddModal] = useState(false);
    const [allDoctors, setAllDoctors] = useState([]);
    const [selectedDoctorId, setSelectedDoctorId] = useState('');

    // Load hidden and added doctor IDs from localStorage on mount
    useEffect(() => {
        const storedHidden = localStorage.getItem('hiddenDoctorIds');
        const storedAdded = localStorage.getItem('addedDoctorIds');

        if (storedHidden) {
            try {
                setHiddenDoctorIds(JSON.parse(storedHidden));
            } catch (e) {
                console.error('Failed to parse hidden doctor IDs from localStorage', e);
                setHiddenDoctorIds([]);
            }
        } else {
            setHiddenDoctorIds([]);
        }

        if (storedAdded) {
            try {
                setAddedDoctorIds(JSON.parse(storedAdded));
            } catch (e) {
                console.error('Failed to parse added doctor IDs from localStorage', e);
                setAddedDoctorIds([]);
            }
        } else {
            setAddedDoctorIds([]);
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

                await populateDoctors(role);
            }
            setLoading(false);
        };

        initialize();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [storageLoaded]);

    const handleViewVisits = (doctorId) => {
        // Navigate to visits page filtered by patient and doctor
        navigate(`/visiturl?patientId=${patientId}&doctorId=${doctorId}`);
    }

    const handleRemoveDoctor = (doctorId, isAddedDoctor) => {
        // Frontend-only removal - filter out the doctor from the displayed list
        // This does NOT delete the backend relationship or any visit history
        const message = isAddedDoctor
            ? 'Remove this doctor from your list?'
            : 'Remove this doctor from your list? (Visit history will be preserved)';

        if (window.confirm(message)) {
            if (isAddedDoctor) {
                // Remove from added list
                const newAddedIds = addedDoctorIds.filter(id => id !== doctorId);
                setAddedDoctorIds(newAddedIds);
                localStorage.setItem('addedDoctorIds', JSON.stringify(newAddedIds));
            } else {
                // Add to hidden list
                const newHiddenIds = [...hiddenDoctorIds, doctorId];
                setHiddenDoctorIds(newHiddenIds);
                localStorage.setItem('hiddenDoctorIds', JSON.stringify(newHiddenIds));
            }

            // Update displayed doctors
            setDoctors(Doctors.filter(doc => doc.doctorId !== doctorId));
        }
    }

    const handleShowAddModal = async () => {
        // Fetch all doctors for the dropdown
        try {
            const token = await authService.getAccessToken();
            const response = await fetch('doctor', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                credentials: 'include'
            });

            if (response.ok) {
                const doctors = await response.json();
                setAllDoctors(doctors);
                setShowAddModal(true);
            }
        } catch (error) {
            console.error('Failed to fetch all doctors:', error);
        }
    }

    const handleAddDoctor = async () => {
        if (!selectedDoctorId) return;

        const doctorId = parseInt(selectedDoctorId);

        // Check if already in the list (backend or added)
        const currentDoctorIds = Doctors.map(d => d.doctorId);
        if (currentDoctorIds.includes(doctorId)) {
            alert('This doctor is already in your list');
            return;
        }

        // Add to localStorage
        const newAddedIds = [...addedDoctorIds, doctorId];
        setAddedDoctorIds(newAddedIds);
        localStorage.setItem('addedDoctorIds', JSON.stringify(newAddedIds));

        // Remove from hidden list if it was there
        if (hiddenDoctorIds.includes(doctorId)) {
            const newHiddenIds = hiddenDoctorIds.filter(id => id !== doctorId);
            setHiddenDoctorIds(newHiddenIds);
            localStorage.setItem('hiddenDoctorIds', JSON.stringify(newHiddenIds));
        }

        // Find the doctor and add to display
        const doctorToAdd = allDoctors.find(d => d.doctorId === doctorId);
        if (doctorToAdd) {
            setDoctors([...Doctors, { ...doctorToAdd, isAdded: true }]);
        }

        // Close modal and reset
        setShowAddModal(false);
        setSelectedDoctorId('');
    }

    // Security implementation: Patients fetch their own patient record using currentUserOnly=true query parameter,
    // which ensures the backend only returns their own patient entity. This prevents exposure of other patients' data
    // while still allowing patients to find their patientId and retrieve their associated doctors.
    const populateDoctors = async (role) => {
        try {
            const token = await authService.getAccessToken();
            let response;

            if (role === 'Patient') {
                // Fetch current patient entity using backend filtering (currentUserOnly=true)
                const headers = !token ? {} : { 'Authorization': `Bearer ${token}` };
                const patientResponse = await fetch('patient?currentUserOnly=true', {
                    headers: headers,
                    credentials: 'include'
                });

                if (!patientResponse.ok) {
                    console.error("Failed to fetch current patient:", patientResponse.status, patientResponse.statusText);
                    setDoctors([]);
                    return;
                }

                let patients;
                try {
                    patients = await patientResponse.json();
                } catch (e) {
                    console.error("Failed to parse patients JSON:", e);
                    setDoctors([]);
                    return;
                }

                // Should only have one patient (the current user)
                const currentPatient = patients[0];

                if (currentPatient) {
                    // Store patientId for later use
                    setPatientId(currentPatient.patientId);

                    // Fetch doctors for this patient
                    response = await fetch(`patient/${currentPatient.patientId}/doctor`, {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (!response.ok) {
                        console.error("Failed to fetch doctors:", response.status, response.statusText);
                        setDoctors([]);
                        return;
                    }
                } else {
                    setDoctors([]);
                    return;
                }
            } else if (role === 'Doctor') {
                // Doctors shouldn't see this page, but if they do, show empty
                setDoctors([]);
                return;
            } else {
                // For other roles, show all doctors
                response = await fetch('doctor', {
                    method: 'GET',
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });
            }

            if (response && response.ok) {
                try {
                    const data = await response.json();
                    // Filter out hidden doctors based on localStorage
                    const visibleDoctors = data.filter(doc => !hiddenDoctorIds.includes(doc.doctorId));

                    // Merge with added doctors from localStorage
                    await mergeAddedDoctors(visibleDoctors);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    setDoctors([]);
                }
            }
        } catch (error) {
            console.error("Error fetching doctors:", error);
        }
    }

    const mergeAddedDoctors = async (backendDoctors) => {
        try {
            // If no added doctors, just set backend doctors
            if (!addedDoctorIds || addedDoctorIds.length === 0) {
                setDoctors(backendDoctors);
                return;
            }

            // Fetch details for added doctors
            const token = await authService.getAccessToken();
            const addedDoctorDetails = [];

            for (const doctorId of addedDoctorIds) {
                // Skip if already in backend doctors or hidden
                if (backendDoctors.find(d => d.doctorId === doctorId) || hiddenDoctorIds.includes(doctorId)) {
                    continue;
                }

                const response = await fetch(`doctor/${doctorId}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });

                if (response.ok) {
                    const doctor = await response.json();
                    addedDoctorDetails.push({ ...doctor, isAdded: true });
                }
            }

            // Combine backend and added doctors
            setDoctors([...backendDoctors, ...addedDoctorDetails]);
        } catch (error) {
            console.error('Error merging added doctors:', error);
            setDoctors(backendDoctors);
        }
    }

    const tableRows = Doctors.map((val, key) => {
        return (
            <tr key={key}>
                <td>
                    {val.doctorName}
                    {val.isAdded && <span className="badge bg-info ms-2">Added</span>}
                </td>
                <td>{val.doctorEmail}</td>
                <td>
                    <button
                        className="btn btn-primary btn-sm"
                        onClick={() => handleViewVisits(val.doctorId)}
                    >
                        View Visits
                    </button>
                </td>
                <td>
                    <button
                        className="btn btn-danger btn-sm"
                        onClick={() => handleRemoveDoctor(val.doctorId, val.isAdded)}
                    >
                        Remove
                    </button>
                </td>
            </tr>
        );
    });

    // Filter available doctors for the dropdown (exclude already displayed ones)
    const availableDoctors = allDoctors.filter(doc => {
        const isDisplayed = Doctors.find(d => d.doctorId === doc.doctorId);
        return !isDisplayed;
    });

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <div className="Doctors">
                <h1>Doctors</h1>
                <p>Please log in to view doctors.</p>
            </div>
        );
    }

    return (
        <div className="Doctors">
            <h1>Doctors</h1>

            {userRole === 'Patient' && Doctors.length === 0 && (
                <p>No doctors assigned yet. Doctors will be automatically assigned when you first log in.</p>
            )}

            {userRole === 'Doctor' && (
                <p>This page is for patients only.</p>
            )}

            {Doctors.length > 0 && (
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
                        Add Doctor
                    </button>
                </>
            )}

            {Doctors.length === 0 && userRole === 'Patient' && (
                <button className="btn btn-success mt-3" onClick={handleShowAddModal}>
                    Add Doctor
                </button>
            )}

            {/* Add Doctor Modal */}
            {showAddModal && (
                <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Add Doctor</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={() => setShowAddModal(false)}
                                ></button>
                            </div>
                            <div className="modal-body">
                                <label htmlFor="doctorSelect" className="form-label">Select a doctor:</label>
                                <select
                                    id="doctorSelect"
                                    className="form-select"
                                    value={selectedDoctorId}
                                    onChange={(e) => setSelectedDoctorId(e.target.value)}
                                >
                                    <option value="">-- Choose a doctor --</option>
                                    {availableDoctors.map(doc => (
                                        <option key={doc.doctorId} value={doc.doctorId}>
                                            {doc.doctorName} ({doc.doctorEmail})
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
                                    onClick={handleAddDoctor}
                                    disabled={!selectedDoctorId}
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

export default Doctors;
