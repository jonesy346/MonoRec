import React, { useEffect, useState } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Table } from 'reactstrap';
import './Home.css';

function Home() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [user, setUser] = useState(null);
    const [userRole, setUserRole] = useState(null);
    const [userId, setUserId] = useState(null);
    const [upcomingVisits, setUpcomingVisits] = useState([]);
    const [patients, setPatients] = useState({});
    const [doctors, setDoctors] = useState({});
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const initialize = async () => {
            const authenticated = await authService.isAuthenticated();
            setIsAuthenticated(authenticated);

            if (authenticated) {
                const currentUser = await authService.getUser();
                setUser(currentUser);
                setUserId(currentUser.sub);

                // Extract role from user claims
                const role = currentUser.role || 'User';
                setUserRole(role);

                await fetchUpcomingVisits();
            }
            setLoading(false);
        };

        initialize();
    }, []);

    const fetchUpcomingVisits = async () => {
        try {
            const token = await authService.getAccessToken();
            const response = await fetch('visit/upcoming', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                const visits = await response.json();
                setUpcomingVisits(visits);

                // Fetch patient and doctor details for each visit
                await fetchPatientAndDoctorDetails(visits);
            }
        } catch (error) {
            console.error('Error fetching upcoming visits:', error);
        }
    };

    const fetchPatientAndDoctorDetails = async (visits) => {
        const token = await authService.getAccessToken();
        const patientIds = [...new Set(visits.map(v => v.patientId))];
        const doctorIds = [...new Set(visits.map(v => v.doctorId))];

        const patientMap = {};
        const doctorMap = {};

        for (const id of patientIds) {
            try {
                const response = await fetch(`patient/${id}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
                });
                if (response.ok) {
                    const patient = await response.json();
                    patientMap[id] = patient;
                }
            } catch (error) {
                console.error(`Error fetching patient ${id}:`, error);
            }
        }

        for (const id of doctorIds) {
            try {
                const response = await fetch(`doctor/${id}`, {
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
                });
                if (response.ok) {
                    const doctor = await response.json();
                    doctorMap[id] = doctor;
                }
            } catch (error) {
                console.error(`Error fetching doctor ${id}:`, error);
            }
        }

        setPatients(patientMap);
        setDoctors(doctorMap);
    };

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <div className="home-container">
            <h1>Welcome to MonoRec</h1>
            <p className="subtitle">Medical Records Management System</p>

            {isAuthenticated ? (
                <div>
                    <div className="user-info-card">
                        <h3>Your Information</h3>
                        <div className="info-row">
                            <span className="info-label">Username:</span>
                            <span className="info-value">{user?.name || 'N/A'}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">User ID:</span>
                            <span className="info-value">{userId || 'N/A'}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Role:</span>
                            <span className="info-value role-badge">{userRole || 'User'}</span>
                        </div>
                    </div>

                    <div className="upcoming-visits-section">
                        <h3>Upcoming Visits</h3>
                        {upcomingVisits.length > 0 ? (
                            <Table striped bordered hover responsive>
                                <thead>
                                    <tr>
                                        <th>Visit ID</th>
                                        <th>Date & Time</th>
                                        <th>Patient</th>
                                        <th>Doctor</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {upcomingVisits.map((visit) => (
                                        <tr key={visit.visitId}>
                                            <td>{visit.visitId}</td>
                                            <td>{formatDate(visit.visitDate)}</td>
                                            <td>{patients[visit.patientId]?.patientName || `Patient #${visit.patientId}`}</td>
                                            <td>{doctors[visit.doctorId]?.doctorName || `Doctor #${visit.doctorId}`}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                        ) : (
                            <p className="no-visits-message">No upcoming visits scheduled.</p>
                        )}
                    </div>
                </div>
            ) : (
                <div className="login-prompt">
                    <p>Please log in to view your information and upcoming visits.</p>
                </div>
            )}
        </div>
    );
}

export default Home;
