import { User } from "../Types/UserTypes";
import { AuthorizedUserData } from "./AuthorizeView";
import DeleteUserButton from "./DeleteUserButton";


export default function UserList(props: {onEditClick: (user: User) => void, users: User[]}){


  async function handleEditClick(user: User) {
    console.log("User to be edited: ", user.id);
    props.onEditClick(user);
  }

  return <table className="table table-striped" aria-labelledby="tabelLabel">
  <thead>
    <tr>
      <th>ID</th>
      <th>Username</th>
      <th>Email</th>
      <th>Email confirmed</th>
      <th>Roles</th>
      <th>Actions</th>
    </tr>
  </thead>
  <tbody>
    {props.users?.map(user => {
      const isCurrentUser = AuthorizedUserData() === user.email;
      return (<tr key={user.id} className={isCurrentUser ? "current" : ""}>
        <td>{user.id}</td>
        <td>{user.username}</td>
        <td>{user.email}</td>
        <td>{user.emailConfirmed ? "✅" : "❌"}</td>
        {/* <td>{user.roles.map(r => r + ", ")}</td> */}
        <td>{user.roles}</td>
        <td>
          <button disabled={isCurrentUser} onClick={() => handleEditClick(user)}>edit</button>
          <DeleteUserButton user={user} disabled={isCurrentUser}/>
        </td>
      </tr>);
    }
    )}
  </tbody>
</table>;
}