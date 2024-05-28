import { useEffect, useState } from "react";

function RoleManagement() {

  const [roles, setRoles] = useState<Role[]>();

  useEffect(() => {
    getRoleData();
  }, []);

  return (
    <div>
      <h1 id="tabelLabel">Roles</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {roles === undefined ? getLoadingContent() : getRoleManagementContent(roles)}
    </div>
  );


  function getRoleManagementContent(roles: Role[]){
    return (
    <>
      <p>Available Roles:</p>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Rolename</th>
          </tr>
        </thead>
        <tbody>
          {roles.map(r => (
            <tr key={r.id}>
              <td>{r.id}</td>
              <td>{r.name}</td>
            </tr>))}
        </tbody>
      </table>
    </>
    );
  }


  function getLoadingContent() {
    return <p><em>Loading...</em></p>;
  }


  async function getRoleData() {
    const response = await fetch('api/user/roles');
    const data = await response.json();
    console.log("Roles:", data);

    setRoles(data);
  }
}

export default RoleManagement;