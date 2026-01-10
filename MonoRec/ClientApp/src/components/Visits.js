import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import authService from './api-authorization/AuthorizeService'

function Visits() {
    const [searchParams] = useSearchParams();
    const [Visits, setVisits] = useState([]);
    const [loading, setLoading] = useState(true);
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    // Get query parameters
    const patientId = searchParams.get('patientId');
    const doctorId = searchParams.get('doctorId');

    const populateVisits = async () => {
        try {
            const token = await authService.getAccessToken();
            let url = 'visit';

            // If both patientId and doctorId are provided, filter by both
            if (patientId && doctorId) {
                url = `visit/patient/${patientId}/doctor/${doctorId}`;
            } else if (patientId) {
                // Filter by patient only
                url = `visit/patient/${patientId}`;
            } else if (doctorId) {
                // Filter by doctor only
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
            } else {
                console.error("Failed to fetch visits:", response.status);
                setVisits([]);
            }
        } catch (error) {
            console.error("Error fetching visits:", error);
            setVisits([]);
        }
    }

    useEffect(() => {
        const initialize = async () => {
            const authenticated = await authService.isAuthenticated();
            setIsAuthenticated(authenticated);

            if (authenticated) {
                await populateVisits();
            }
            setLoading(false);
        };

        initialize();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [patientId, doctorId]);
    
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

            {Visits.length === 0 ? (
                <p>No visits found.</p>
            ) : (
                <table className="table table-striped table-hover">
                    <thead>
                        <tr>
                            <th>Visit ID</th>
                            <th>Date</th>
                            <th>Patient ID</th>
                            <th>Doctor ID</th>
                        </tr>
                    </thead>
                    <tbody>
                        {Visits.map((val, key) => {
                            return (
                                <tr key={key}>
                                    <td>{val.visitId}</td>
                                    <td>{new Date(val.visitDate).toLocaleDateString()}</td>
                                    <td>{val.patientId}</td>
                                    <td>{val.doctorId}</td>
                                </tr>
                            )
                        })}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default Visits;