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
    const [hiddenDoctorIds, setHiddenDoctorIds] = useState([]);

    // Load hidden doctor IDs from localStorage on mount
    useEffect(() => {
        const stored = localStorage.getItem('hiddenDoctorIds');
        if (stored) {
            try {
                setHiddenDoctorIds(JSON.parse(stored));
            } catch (e) {
                console.error('Failed to parse hidden doctor IDs from localStorage', e);
                setHiddenDoctorIds([]);
            }
        }
    }, []);

    useEffect(() => {
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
    }, [hiddenDoctorIds]);

    const handleViewVisits = (doctorId) => {
        // Navigate to visits page filtered by patient and doctor
        navigate(`/visiturl?patientId=${patientId}&doctorId=${doctorId}`);
    }

    const handleRemoveDoctor = (doctorId) => {
        // Frontend-only removal - filter out the doctor from the displayed list
        // This does NOT delete the backend relationship or any visit history
        if (window.confirm('Remove this doctor from your list? (Visit history will be preserved)')) {
            // Add to hidden list
            const newHiddenIds = [...hiddenDoctorIds, doctorId];
            setHiddenDoctorIds(newHiddenIds);

            // Save to localStorage
            localStorage.setItem('hiddenDoctorIds', JSON.stringify(newHiddenIds));

            // Update displayed doctors
            setDoctors(Doctors.filter(doc => doc.doctorId !== doctorId));
        }
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
                    setDoctors(visibleDoctors);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    setDoctors([]);
                }
            }
        } catch (error) {
            console.error("Error fetching doctors:", error);
        }
    }

    const tableRows = Doctors.map((val, key) => {
        return (
            <tr key={key}>
                <td>{val.doctorName}</td>
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
                        onClick={() => handleRemoveDoctor(val.doctorId)}
                    >
                        Remove
                    </button>
                </td>
            </tr>
        );
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
            )}
        </div>
    );
}

export default Doctors;
