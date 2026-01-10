import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

function Visits() {
    const [searchParams] = useSearchParams();
    const [Visits, setVisits] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [doctorId, setDoctorId] = useState(null);
    const [currentPatientId, setCurrentPatientId] = useState(null);
    const [patients, setPatients] = useState({});
    const [doctors, setDoctors] = useState({});
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [allPatients, setAllPatients] = useState([]);
    const [selectedVisit, setSelectedVisit] = useState(null);

    const [createFormData, setCreateFormData] = useState({
        patientId: '',
        visitDate: new Date().toISOString().split('T')[0],
        visitNote: ''
    });

    const [editFormData, setEditFormData] = useState({
        visitDate: '',
        visitNote: ''
    });

    const patientId = searchParams.get('patientId');
    const doctorIdParam = searchParams.get('doctorId');

    const fetchPatientDetails = async (patientIds) => {
        const token = await authService.getAccessToken();
        const patientDetails = {};

        for (const id of patientIds) {
            try {
                const response = await fetch(`patient/${id}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });

                if (response.ok) {
                    const patient = await response.json();
                    patientDetails[id] = patient;
                }
            } catch (error) {
                console.error(`Error fetching patient ${id}:`, error);
            }
        }

        return patientDetails;
    };

    const fetchDoctorDetails = async (doctorIds) => {
        const token = await authService.getAccessToken();
        const doctorDetails = {};

        for (const id of doctorIds) {
            try {
                const response = await fetch(`doctor/${id}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });

                if (response.ok) {
                    const doctor = await response.json();
                    doctorDetails[id] = doctor;
                }
            } catch (error) {
                console.error(`Error fetching doctor ${id}:`, error);
            }
        }

        return doctorDetails;
    };

    const populateVisits = async () => {
        try {
            const token = await authService.getAccessToken();
            let url = 'visit';

            if (patientId && doctorIdParam) {
                url = `visit/patient/${patientId}/doctor/${doctorIdParam}`;
            } else if (patientId) {
                url = `visit/patient/${patientId}`;
            } else if (currentPatientId) {
                url = `visit/patient/${currentPatientId}`;
            } else if (doctorIdParam) {
                url = `visit/doctor/${doctorIdParam}`;
            } else if (doctorId) {
                url = `visit/doctor/${doctorId}`;
            }

            const response = await fetch(url, {
                method: 'GET',
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                credentials: 'include'
            });

            if (response.ok) {
                const data = await response.json();
                setVisits(data);

                // Fetch patient or doctor details based on user role
                if (userRole === 'Doctor') {
                    const uniquePatientIds = [...new Set(data.map(v => v.patientId))];
                    const patientDetails = await fetchPatientDetails(uniquePatientIds);
                    setPatients(patientDetails);
                } else if (userRole === 'Patient') {
                    const uniqueDoctorIds = [...new Set(data.map(v => v.doctorId))];
                    const doctorDetails = await fetchDoctorDetails(uniqueDoctorIds);
                    setDoctors(doctorDetails);
                }
            } else {
                console.error("Failed to fetch visits:", response.status);
                setVisits([]);
            }
        } catch (error) {
            console.error("Error fetching visits:", error);
            setVisits([]);
        }
    };

    const fetchAllPatients = async () => {
        if (userRole !== 'Doctor') return;

        try {
            const token = await authService.getAccessToken();
            const response = await fetch('patient', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                credentials: 'include'
            });

            if (response.ok) {
                const data = await response.json();
                setAllPatients(data);
            }
        } catch (error) {
            console.error('Error fetching patients:', error);
        }
    };

    useEffect(() => {
        const initialize = async () => {
            const authenticated = await authService.isAuthenticated();
            setIsAuthenticated(authenticated);

            if (authenticated) {
                const currentUser = await authService.getUser();
                const role = currentUser.role || 'User';
                setUserRole(role);

                const token = await authService.getAccessToken();
                const userId = currentUser.sub;

                if (role === 'Doctor') {
                    // Try to load from cache first
                    const cachedDoctorId = localStorage.getItem('currentDoctorId');
                    if (cachedDoctorId) {
                        setDoctorId(parseInt(cachedDoctorId));
                    }

                    // Fetch all doctors and find the one matching this user
                    const response = await fetch('doctor', {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (response.ok) {
                        const doctors = await response.json();
                        const currentDoctor = doctors.find(d => d.userId === userId);

                        if (currentDoctor) {
                            console.log('Doctor loaded:', currentDoctor);
                            setDoctorId(currentDoctor.doctorId);
                            localStorage.setItem('currentDoctorId', currentDoctor.doctorId.toString());
                        } else {
                            console.error('No doctor record found for user:', userId);
                        }
                    } else {
                        console.error('Failed to fetch doctors:', response.status, await response.text());
                    }
                } else if (role === 'Patient') {
                    // Try to load from cache first
                    const cachedPatientId = localStorage.getItem('currentPatientId');
                    if (cachedPatientId) {
                        setCurrentPatientId(parseInt(cachedPatientId));
                    }

                    // Fetch current patient using currentUserOnly parameter
                    const response = await fetch('patient?currentUserOnly=true', {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (response.ok) {
                        const patients = await response.json();
                        const currentPatient = patients[0];

                        if (currentPatient) {
                            console.log('Patient loaded:', currentPatient);
                            setCurrentPatientId(currentPatient.patientId);
                            localStorage.setItem('currentPatientId', currentPatient.patientId.toString());
                        } else {
                            console.error('No patient record found for user:', userId);
                        }
                    } else {
                        console.error('Failed to fetch patient:', response.status, await response.text());
                    }
                }
            }
            setLoading(false);
        };

        initialize();
    }, []);

    useEffect(() => {
        if (isAuthenticated && (doctorId || currentPatientId || patientId || doctorIdParam)) {
            populateVisits();
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isAuthenticated, doctorId, currentPatientId, patientId, doctorIdParam]);

    useEffect(() => {
        if (isAuthenticated && userRole === 'Doctor') {
            fetchAllPatients();
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [isAuthenticated, userRole]);

    const handleShowCreateModal = () => {
        if (!doctorId) {
            alert('Doctor ID not loaded yet. Please wait a moment and try again.');
            return;
        }
        setCreateFormData({
            patientId: '',
            visitDate: new Date().toISOString().split('T')[0],
            visitNote: ''
        });
        setShowCreateModal(true);
    };

    const handleShowEditModal = (visit) => {
        setSelectedVisit(visit);
        setEditFormData({
            visitDate: new Date(visit.visitDate).toISOString().split('T')[0],
            visitNote: visit.visitNote || ''
        });
        setShowEditModal(true);
    };

    const handleCreateVisit = async () => {
        if (!createFormData.patientId) {
            alert('Please select a patient');
            return;
        }

        try {
            const token = await authService.getAccessToken();

            console.log('Creating visit with patientId:', createFormData.patientId, 'doctorId:', doctorId);

            const createResponse = await fetch(`visit/${createFormData.patientId}/${doctorId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(!token ? {} : { 'Authorization': `Bearer ${token}` })
                },
                credentials: 'include'
            });

            if (!createResponse.ok) {
                const errorText = await createResponse.text();
                console.error('Failed to create visit:', createResponse.status, errorText);
                alert(`Failed to create visit: ${createResponse.status} - ${errorText}`);
                return;
            }

            const newVisit = await createResponse.json();

            const updateResponse = await fetch(`visit/${newVisit.visitId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    ...(!token ? {} : { 'Authorization': `Bearer ${token}` })
                },
                credentials: 'include',
                body: JSON.stringify({
                    visitDate: new Date(createFormData.visitDate).toISOString(),
                    visitNote: createFormData.visitNote
                })
            });

            if (updateResponse.ok) {
                setShowCreateModal(false);
                await populateVisits();
            } else {
                alert('Failed to update visit details');
            }
        } catch (error) {
            console.error('Error creating visit:', error);
            alert('An error occurred while creating the visit');
        }
    };

    const handleUpdateVisit = async () => {
        if (!selectedVisit) return;

        try {
            const token = await authService.getAccessToken();
            const response = await fetch(`visit/${selectedVisit.visitId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    ...(!token ? {} : { 'Authorization': `Bearer ${token}` })
                },
                credentials: 'include',
                body: JSON.stringify({
                    visitDate: new Date(editFormData.visitDate).toISOString(),
                    visitNote: editFormData.visitNote
                })
            });

            if (response.ok) {
                setShowEditModal(false);
                await populateVisits();
            } else {
                alert('Failed to update visit');
            }
        } catch (error) {
            console.error('Error updating visit:', error);
            alert('An error occurred while updating the visit');
        }
    };

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <div className="Visits">
                <h1>Visits</h1>
                <p>Please log in to view visits.</p>
            </div>
        );
    }

    return (
        <div className="Visits">
            <h1>Visits</h1>

            {userRole === 'Doctor' && (
                <button
                    className="btn btn-success mb-3"
                    onClick={handleShowCreateModal}
                    disabled={!doctorId}
                >
                    Create New Visit
                </button>
            )}

            {Visits.length === 0 ? (
                <p>No visits found.</p>
            ) : (
                <table className="table table-striped table-hover">
                    <thead>
                        <tr>
                            {userRole === 'Doctor' ? (
                                <>
                                    <th>Patient Name</th>
                                    <th>Patient Email</th>
                                </>
                            ) : (
                                <>
                                    <th>Doctor Name</th>
                                    <th>Doctor Email</th>
                                </>
                            )}
                            <th>Date</th>
                            <th>Visit Note</th>
                            {userRole === 'Doctor' && <th>Actions</th>}
                        </tr>
                    </thead>
                    <tbody>
                        {Visits.map((visit, key) => {
                            if (userRole === 'Doctor') {
                                const patient = patients[visit.patientId];
                                return (
                                    <tr key={key}>
                                        <td>{patient ? patient.patientName : `Patient ${visit.patientId}`}</td>
                                        <td>{patient ? patient.patientEmail : 'N/A'}</td>
                                        <td>{new Date(visit.visitDate).toLocaleDateString()}</td>
                                        <td>{visit.visitNote || 'No notes'}</td>
                                        <td>
                                            <button
                                                className="btn btn-primary btn-sm"
                                                onClick={() => handleShowEditModal(visit)}
                                            >
                                                Edit
                                            </button>
                                        </td>
                                    </tr>
                                );
                            } else {
                                const doctor = doctors[visit.doctorId];
                                return (
                                    <tr key={key}>
                                        <td>{doctor ? doctor.doctorName : `Doctor ${visit.doctorId}`}</td>
                                        <td>{doctor ? doctor.doctorEmail : 'N/A'}</td>
                                        <td>{new Date(visit.visitDate).toLocaleDateString()}</td>
                                        <td>{visit.visitNote || 'No notes'}</td>
                                    </tr>
                                );
                            }
                        })}
                    </tbody>
                </table>
            )}

            {showCreateModal && (
                <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Create New Visit</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={() => setShowCreateModal(false)}
                                ></button>
                            </div>
                            <div className="modal-body">
                                <div className="mb-3">
                                    <label htmlFor="patientSelect" className="form-label">Patient:</label>
                                    <select
                                        id="patientSelect"
                                        className="form-select"
                                        value={createFormData.patientId}
                                        onChange={(e) => setCreateFormData({ ...createFormData, patientId: e.target.value })}
                                    >
                                        <option value="">-- Select a patient --</option>
                                        {allPatients.map(patient => (
                                            <option key={patient.patientId} value={patient.patientId}>
                                                {patient.patientName} ({patient.patientEmail})
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="visitDate" className="form-label">Visit Date:</label>
                                    <input
                                        type="date"
                                        id="visitDate"
                                        className="form-control"
                                        value={createFormData.visitDate}
                                        onChange={(e) => setCreateFormData({ ...createFormData, visitDate: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="visitNote" className="form-label">Visit Note:</label>
                                    <textarea
                                        id="visitNote"
                                        className="form-control"
                                        rows="4"
                                        value={createFormData.visitNote}
                                        onChange={(e) => setCreateFormData({ ...createFormData, visitNote: e.target.value })}
                                        placeholder="Enter symptoms, diagnosis, prescriptions, etc."
                                    ></textarea>
                                </div>
                            </div>
                            <div className="modal-footer">
                                <button
                                    type="button"
                                    className="btn btn-secondary"
                                    onClick={() => setShowCreateModal(false)}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={handleCreateVisit}
                                    disabled={!createFormData.patientId}
                                >
                                    Create
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {showEditModal && selectedVisit && (
                <div className="modal show d-block" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Edit Visit</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={() => setShowEditModal(false)}
                                ></button>
                            </div>
                            <div className="modal-body">
                                <div className="mb-3">
                                    <label className="form-label">Patient:</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={patients[selectedVisit.patientId]?.patientName || `Patient ${selectedVisit.patientId}`}
                                        disabled
                                    />
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="editVisitDate" className="form-label">Visit Date:</label>
                                    <input
                                        type="date"
                                        id="editVisitDate"
                                        className="form-control"
                                        value={editFormData.visitDate}
                                        onChange={(e) => setEditFormData({ ...editFormData, visitDate: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label htmlFor="editVisitNote" className="form-label">Visit Note:</label>
                                    <textarea
                                        id="editVisitNote"
                                        className="form-control"
                                        rows="4"
                                        value={editFormData.visitNote}
                                        onChange={(e) => setEditFormData({ ...editFormData, visitNote: e.target.value })}
                                        placeholder="Enter symptoms, diagnosis, prescriptions, etc."
                                    ></textarea>
                                </div>
                            </div>
                            <div className="modal-footer">
                                <button
                                    type="button"
                                    className="btn btn-secondary"
                                    onClick={() => setShowEditModal(false)}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={handleUpdateVisit}
                                >
                                    Save Changes
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Visits;
