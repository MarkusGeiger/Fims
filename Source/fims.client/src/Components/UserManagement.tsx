import { useEffect, useState } from "react";
import { AuthorizedUserData } from "./AuthorizeView";
import "./UserManagement.css"
import RoleManagement from "./RoleManagement";
import DeleteUserButton from "./DeleteUserButton";
import UserEdit from "./UserEdit";
import { User } from "../Types/UserTypes";
import UserList from "./UserList";

function UserManagement() {
  const [users, setUsers] = useState<User[]>();
  const [editUser, setEditUser] = useState<User>();

  useEffect(() => {
    getUserData();
  }, []);

  async function handleEditClick(user: User) {
    console.log("User to be edited: ", user.id);
    setEditUser(user);
  }

  function getUserManagementContent() {
    return <>
      {editUser == undefined || editUser == null ? getUserList() : getUserEditComponent() }
    </>
  }

  function onUserEditSaved(){
    console.log("UserEdit saved in UserManagement");
    
    setEditUser(undefined);
    getUserData();
  }

  function getUserEditComponent(){
    return <>
      <div>
        <UserEdit user={editUser!} onCancel={() => setEditUser(undefined)} onSave={onUserEditSaved}/>
      </div>
    </>
  }

  function getUserList(){
    return <UserList onEditClick={handleEditClick} users={users ?? []}/>
  }

  return (
    <div>
      <h1 id="tabelLabel">Users</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {users === undefined ? getLoadingContent() : getUserManagementContent()}
      { <RoleManagement />}
    </div>
  );

  function getLoadingContent() {
    return <p><em>Loading...</em></p>;
  }

  async function getUserData() {
    const response = await fetch('api/user');
    const data = await response.json();
    console.log("Userdata:", data);

    setUsers(data);
  }

}

export default UserManagement;

