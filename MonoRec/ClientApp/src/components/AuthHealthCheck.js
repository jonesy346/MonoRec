import React, { Component } from 'react';
import authService from './api-authorization/AuthorizeService';

export class AuthHealthCheck extends Component {
  static displayName = AuthHealthCheck.name;

  constructor(props) {
    super(props);
    this.state = { userInfo: null, loading: true, error: null };
  }

  async componentDidMount() {
    try {
      // Always try the API endpoint first, regardless of authService state
      // This is because cookie auth might work even if authService doesn't recognize it
      const response = await fetch('/api/users/me', {
        credentials: 'include'
      });

      if (response.ok) {
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
          const data = await response.json();

          // Check if the API says we're authenticated
          if (data.authenticated) {
            this.setState({ userInfo: data, loading: false });
            return;
          }
        }
      }

      // If API didn't work, fall back to authService
      const isAuthenticated = await authService.isAuthenticated();

      if (isAuthenticated) {
        const user = await authService.getUser();

        // Try to extract roles from various possible claim locations
        const roles = user.role ? [user.role] :
                     (user.roles ? (Array.isArray(user.roles) ? user.roles : [user.roles]) :
                     (user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?
                      (Array.isArray(user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']) ?
                       user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] :
                       [user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']]) : []));

        this.setState({
          userInfo: {
            authenticated: true,
            userId: user.sub,
            email: user.email || user.name,
            name: user.name,
            roles: roles
          },
          loading: false
        });
      } else {
        this.setState({
          userInfo: { authenticated: false },
          loading: false
        });
      }
    } catch (error) {
      this.setState({ error: error.message, loading: false });
    }
  }

  render() {
    const { userInfo, loading, error } = this.state;

    if (loading) {
      return <div style={styles.container}><p>Loading...</p></div>;
    }

    if (error) {
      return (
        <div style={styles.container}>
          <h1 style={styles.title}>🔐 Auth Health Check</h1>
          <div style={styles.errorBox}>
            <h2>❌ Error</h2>
            <p>{error}</p>
          </div>
        </div>
      );
    }

    if (!userInfo || !userInfo.authenticated) {
      return (
        <div style={styles.container}>
          <h1 style={styles.title}>🔐 Auth Health Check</h1>
          <div style={styles.errorBox}>
            <h2>❌ Not Authenticated</h2>
            <p>You are not currently logged in.</p>
            <p><a href="/Identity/Account/Login">Click here to log in</a></p>
          </div>
        </div>
      );
    }

    return (
      <div style={styles.container}>
        <h1 style={styles.title}>🔐 Auth Health Check</h1>
        <div style={styles.successBox}>
          <h2 style={styles.successTitle}>✅ Authenticated</h2>
          <div style={styles.infoBox}>
            {userInfo.userId && <p><span style={styles.label}>User ID:</span><span style={styles.value}>{userInfo.userId}</span></p>}
            {userInfo.email && <p><span style={styles.label}>Email:</span><span style={styles.value}>{userInfo.email}</span></p>}
            {userInfo.name && <p><span style={styles.label}>Name:</span><span style={styles.value}>{userInfo.name}</span></p>}
            {userInfo.roles && userInfo.roles.length > 0 && (
              <p>
                <span style={styles.label}>{userInfo.roles.length === 1 ? 'Role:' : 'Roles:'}</span>
                <span style={styles.value}>{userInfo.roles.join(', ')}</span>
              </p>
            )}
          </div>
        </div>
      </div>
    );
  }
}

const styles = {
  container: {
    fontFamily: 'Arial, sans-serif',
    maxWidth: '800px',
    margin: '50px auto',
    padding: '20px'
  },
  title: {
    color: '#333'
  },
  successBox: {
    padding: '20px',
    borderRadius: '5px',
    margin: '20px 0',
    backgroundColor: '#efe',
    border: '2px solid #3c3'
  },
  errorBox: {
    padding: '20px',
    borderRadius: '5px',
    margin: '20px 0',
    backgroundColor: '#fee',
    border: '2px solid #c33'
  },
  successTitle: {
    color: '#2c5',
    marginTop: 0
  },
  infoBox: {
    backgroundColor: '#f5f5f5',
    padding: '15px',
    borderRadius: '5px',
    margin: '10px 0'
  },
  label: {
    fontWeight: 'bold',
    color: '#666'
  },
  value: {
    color: '#000',
    marginLeft: '10px'
  }
};
